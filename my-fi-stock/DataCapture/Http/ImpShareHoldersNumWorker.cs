using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using System.Threading;

using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Filters;

using Pandora.Invest.MThread;
using Pandora.Basis.DB;
using Pandora.Basis.Utils;
using Pandora.Invest.Entity;

namespace Pandora.Invest.DataCapture.Http
{
    /// <summary>
    /// 从http://stock.finance.qq.com/corp1/stk_holder_count.php?zqdm=600111抓取股东数<br />
    /// 返回的html内容参考GetStockHolderCount_qq.html
    /// </summary>
    public class ImpShareHoldersNumWorker : MThreadWorker<Stock>{
        private const string URL = "http://stock.finance.qq.com/corp1/stk_holder_count.php?zqdm={0}";
        private const string ENCODING = "utf-8";
        private const int MAX_RETRIES = 3;

        private Database _db;

        public override string LogPrefix {
            get {
                return "imp-holder";
            }
        }

        public override void BeforeDo(MThreadContext context)
        {
            string connectionString = context.Get("connection-string").ToString();
            this._db = new Database(connectionString);
            this._db.Open();
        }

        public override void AfterDo(MThreadContext context)
        {
            this._db.Close();
        }

        public override void Do(MThreadContext context, Stock stock){
            DateTime start = DateTime.Now;

            string html = string.Empty;
            try{
                html = HttpUtil.HttpGet(string.Format(URL, stock.StockCode), ENCODING, MAX_RETRIES, 1000, false);
            } catch (Exception ex){
                Error(MAX_RETRIES + " HTTP retries failed, Ignored: " + ex.Message, ex);
                return;
            }

            DateTime httpget = DateTime.Now;

            //解析html
            Lexer lexer = new Lexer(html);
            Parser parser = new Parser(lexer);
            NodeList nodes = parser.Parse(new AndFilter(new NodeClassFilter(typeof(TableTag)), new HasAttributeFilter("CLASS")));
            INode table = FindHolderCountNode(nodes);
            if (table == null || table.Children == null || table.Children.Count <= 0)
                return;

            DateTime parse = DateTime.Now;

            int trIndex = -1;
            IList<ShareholdersNumEntity> entities = new List<ShareholdersNumEntity>();
            for (int i = 0; i < table.Children.Count; i++){
                INode node = table.Children[i];
                if (!(node is ITag))
                    continue;
                ITag tag = node as ITag;
                if (tag.IsEndTag() || tag.TagName.Trim().ToLower() != "tr")
                    continue;
                trIndex++;
                if (trIndex == 0)
                    continue; //第一行为标题行，忽略

                string s = GetTdText(tag, 0);
                DateTime date = DateTime.MinValue;
                DateTime.TryParse(s, out date);
                if (date <= DateTime.MinValue)
                    continue;

                int holderCount = ConvertInt(GetTdText(tag, 1), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd") + ", 无效股东数");
                int avgNum = ConvertInt(GetTdText(tag, 2), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd") + ", 无效户均持股数");
                int totalShares = ConvertInt(GetTdText(tag, 3), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd") + ", 无效总股本");
                int transShares = ConvertInt(GetTdText(tag, 4), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd") + ", 无效流通股本");

                ShareholdersNumEntity entity = new ShareholdersNumEntity()
                {
                    StockId = stock.StockId,
                    ReportDate = date,
                    HoldersNum = holderCount,
                    AvgShares = avgNum,
                    TotalShares = totalShares,
                    TransShares = transShares,
                    Source = "qq"
                };
                entities.Add(entity);
            }
            int insertedRows = ShareholdersNumEntity.Create(this._db, entities);

            TimeSpan ts1 = httpget - start, ts2 = parse - httpget, ts3 = DateTime.Now - parse;
            Info("Rows: http=" + entities.Count + ", db=" + insertedRows + "; Elapsed http=" +
                (ts1.TotalMilliseconds / 1000.0).ToString("f2") + "s, parse=" +
                (ts2.TotalMilliseconds / 1000.0).ToString("f2") + "s, db=" +
                (ts3.TotalMilliseconds / 1000.0).ToString("f2") + "s");
        }

        /// <summary>
        /// 找table节点，且class="list list_d"
        /// </summary>
        /// <param name="nodes"></param>
        /// <returns></returns>
        private INode FindHolderCountNode(NodeList nodes){
            if(nodes==null || nodes.Count<=0) return null;
            for(int i=0; i<nodes.Count; i++){
                INode node = nodes[i];
                if(!(node is ITag)) continue;
                ITag tag = node as ITag;
                if(tag.IsEndTag()) continue;
                if(tag.TagName.Trim().ToLower()=="table") { //找table节点
                    if(tag.Attributes!=null && tag.Attributes.Count>0 
                        && tag.Attributes["CLASS"]!=null
                        && tag.Attributes["CLASS"].ToString().Trim().ToLower()=="list list_d"){
                        return node;
                    }
                }
                INode found = FindHolderCountNode(tag.Children);
                if(found!=null) return found;
            }
            return null;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="tdIndex">从0开始的位置索引</param>
        /// <returns></returns>
        private string GetTdText(ITag parent, int tdIndex){
            if(parent==null || parent.Children==null || parent.Children.Count<=0) return string.Empty;
            int index = 0;
            for(int i=0; i<parent.Children.Count; i++){
                INode node = parent.Children[i];
                if(!(node is ITag)) continue;
                ITag tag = node as ITag;
                if(tag.IsEndTag()) continue;
                if(tag.TagName.Trim().ToLower()!="td" && tag.TagName.Trim().ToLower()!="th") continue;
                if(index < tdIndex) {
                    index++;
                    continue;
                }
                return tag.ToPlainTextString().Trim(' ', '\r', '\n', '\t');
            }
            return string.Empty;
        }

        private int ConvertInt(string str, string message){
            if(string.IsNullOrEmpty(str)) return 0;
            str = str.Replace(",","").Replace("-","");
            if(string.IsNullOrEmpty(str)) return 0;
            try{
                return Convert.ToInt32(Convert.ToDecimal(str));
            }catch{
                Info(message);
            }
            return 0;
        }
    }
}