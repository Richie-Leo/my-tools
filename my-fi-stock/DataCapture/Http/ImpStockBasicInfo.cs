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

using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using Pandora.Basis.Utils;
using Pandora.Invest.MThread;

namespace Pandora.Invest.DataCapture.Http
{
    public class ImpStockInfoWorker : MThreadWorker<Stock>
	{
		private const string URL = "http://gu.qq.com/{0}{1}";
		private const string ENCODING = "utf-8";
		private const int MAX_RETRIES = 3;
		private const int THREAD_NUM = 6;

        private Database _db;

        public override string LogPrefix {
            get {
                return "imp-info";
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
                html = HttpUtil.HttpGet(string.Format(URL, stock.StockCode[0] == '6' ? "sh" : "sz", stock.StockCode), ENCODING, MAX_RETRIES, 1000, false);
            } catch (Exception ex){
                Error(MAX_RETRIES + " HTTP retries failed, Ignored: " + ex.Message, ex);
                return;
            }

            DateTime httpget = DateTime.Now;

            //解析html
            int p1 = html.IndexOf("<table class=\"data\">");
            if (p1 < 0){
                Info("返回的HTML格式错误，未发现 公司概况 节点：<table class=\"data\">");
                return;
            }
            int p2 = html.IndexOf("</table>", p1);
            string tableStr = html.Substring(p1, p2 - p1 + "</table>".Length);

            Lexer lexer = new Lexer(tableStr);
            Parser parser = new Parser(lexer);
            NodeList nodes = parser.Parse(null);
            if (nodes == null || nodes.Count <= 0){
                Info("HTMLParser未解析出table节点");
                return;
            }

            DateTime parse = DateTime.Now;

            string text, value;
            decimal dec;
            INode table = nodes[0];

            ITag tr = this.GetTr(table, 0); //第1行
            //所属地区
            stock.CompanyLocation = this.GetTdText(tr, 1);
            //总股本
            text = this.GetTdText(tr, 2);
            value = this.GetTdText(tr, 3);
            dec = ConvertUtil.ToDecimal(value, 0);
            if (text.IndexOf('亿') >= 0)
                stock.TotalCapital = Convert.ToInt64(dec * 100000000);
            else if (text.IndexOf('万') >= 0)
                stock.TotalCapital = Convert.ToInt64(dec * 10000);
            else
                stock.TotalCapital = Convert.ToInt64(dec);

            tr = GetTr(table, 1); //第2行
            //上市时间
            stock.ListDate = ConvertUtil.ToDateTime(this.GetTdText(tr, 1), stock.ListDate);
            //流通股本
            text = this.GetTdText(tr, 2);
            value = this.GetTdText(tr, 3);
            dec = ConvertUtil.ToDecimal(value, 0);
            if (text.IndexOf('亿') >= 0)
                stock.CirculatingCapital = Convert.ToInt64(dec * 100000000);
            else if (text.IndexOf('万') >= 0)
                stock.CirculatingCapital = Convert.ToInt64(dec * 10000);
            else
                stock.CirculatingCapital = Convert.ToInt64(dec);
            //每股净资产
            stock.NetAssetValuePerShare = ConvertUtil.ToDecimal(this.GetTdText(tr, 5), stock.NetAssetValuePerShare);

            tr = this.GetTr(table, 3); //第4行
            stock.NetProfitGrowth = ConvertUtil.ToDecimal(this.GetTdText(tr, 1), stock.NetProfitGrowth);

            stock.UpdateInfo(this._db);

            TimeSpan ts1 = httpget - start, ts2 = parse - httpget, ts3 = DateTime.Now - parse;
            Info("Elapsed: http=" +
                (ts1.TotalMilliseconds / 1000.0).ToString("f2") + "s, parse=" +
                (ts2.TotalMilliseconds / 1000.0).ToString("f2") + "s, db=" +
                (ts3.TotalMilliseconds / 1000.0).ToString("f2") + "s");
        }
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="trIndex">从0开始的位置索引</param>
		/// <returns></returns>
		private ITag GetTr(INode parent, int trIndex){
			if(parent==null || parent.Children==null || parent.Children.Count<=0) return null;
			int index = 0;
			for(int i=0; i<parent.Children.Count; i++){
				INode node = parent.Children[i];
				if(!(node is ITag)) continue;
				ITag tag = node as ITag;
				if(tag.IsEndTag()) continue;
				if(tag.TagName.Trim().ToLower()!="tr") continue;
				if(index < trIndex) {
					index++;
					continue;
				}
				return tag;
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