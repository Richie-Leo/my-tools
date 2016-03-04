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
    public class ImpStockBasInfoWorker : MThreadWorker<Stock>
	{
		private const string URL = "http://gu.qq.com/{0}{1}";
        private const string URL2 = "http://stock.finance.qq.com/corp1/profile.php?zqdm={0}";
		private const int MAX_RETRIES = 3;

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
            #region 获取股本、净资产等
            try{
                html = HttpUtil.HttpGet(string.Format(URL, stock.StockCode[0] == '6' ? "sh" : "sz", stock.StockCode), "utf-8", MAX_RETRIES, 1000, true);
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

            string text, value;
            decimal dec;
            INode table = nodes[0];

            ITag tr = HtmlUtil.GetTr(table, 0); //第1行
            //所属地区
            stock.CompanyLocation = HtmlUtil.GetTdText(tr, 1);
            //总股本
            text = HtmlUtil.GetTdText(tr, 2);
            value = HtmlUtil.GetTdText(tr, 3);
            dec = ConvertUtil.ToDecimal(value, 0);
            if (text.IndexOf('亿') >= 0)
                stock.TotalCapital = Convert.ToInt64(dec * 100000000);
            else if (text.IndexOf('万') >= 0)
                stock.TotalCapital = Convert.ToInt64(dec * 10000);
            else
                stock.TotalCapital = Convert.ToInt64(dec);

            tr = HtmlUtil.GetTr(table, 1); //第2行
            //上市时间
            stock.ListDate = ConvertUtil.ToDateTime(HtmlUtil.GetTdText(tr, 1), stock.ListDate);
            //流通股本
            text = HtmlUtil.GetTdText(tr, 2);
            value = HtmlUtil.GetTdText(tr, 3);
            dec = ConvertUtil.ToDecimal(value, 0);
            if (text.IndexOf('亿') >= 0)
                stock.CirculatingCapital = Convert.ToInt64(dec * 100000000);
            else if (text.IndexOf('万') >= 0)
                stock.CirculatingCapital = Convert.ToInt64(dec * 10000);
            else
                stock.CirculatingCapital = Convert.ToInt64(dec);
            //每股净资产
            stock.NetAssetValuePerShare = ConvertUtil.ToDecimal(HtmlUtil.GetTdText(tr, 5), stock.NetAssetValuePerShare);

            tr = HtmlUtil.GetTr(table, 3); //第4行
            stock.NetProfitGrowth = ConvertUtil.ToDecimal(HtmlUtil.GetTdText(tr, 1), stock.NetProfitGrowth);
            DateTime parse = DateTime.Now;
            #endregion

            #region 获取行业、板块
            html = string.Empty;
            tableStr = string.Empty;
            try{
                html = HttpUtil.HttpGet(string.Format(URL2, stock.StockCode), "gb2312", MAX_RETRIES, 1000, true);
            } catch (Exception ex){
                Error(MAX_RETRIES + " HTTP retries failed, Ignored: " + ex.Message, ex);
                return;
            }
            if(!string.IsNullOrEmpty(html)){
                p1 = html.IndexOf("<table");
                if(p1>0){
                    p1 = html.IndexOf("<table", p1 + 1);
                    if(p1>0){
                        p2 = html.IndexOf("</table>", p1 + 1);
                        tableStr = html.Substring(p1, p2 - p1 + "</table>".Length);
                    }
                }
            }
            if(!string.IsNullOrEmpty(tableStr)){
                lexer = new Lexer(tableStr);
                parser = new Parser(lexer);
                nodes = parser.Parse(null);
                table = nodes[0];
                if (nodes != null && nodes.Count > 0){
                    tr = HtmlUtil.GetTr(table, 4); //所属行业所在tr
                    if(tr!=null)
                        stock.Industry = HtmlUtil.GetTdText(tr, 1).Trim(' ', '"', '\t', '\n', '\r');

                    tr = HtmlUtil.GetTr(table, 16); //所属板块所在tr
                    if(tr!=null)
                        stock.Plate = "-" + HtmlUtil.GetTdText(tr, 1).Trim(' ', '"', '\t', '\n', '\r').Replace(' ', '-') + "-";
                }
            }
            #endregion
            DateTime info2 = DateTime.Now;

            stock.UpdateInfo(this._db);

            TimeSpan ts1 = httpget - start, ts2 = parse - httpget, ts3 = info2 - parse, ts4 = DateTime.Now - info2;
            Info("Elapsed " + ((DateTime.Now - start).TotalMilliseconds / 1000.0).ToString("f1") + "s, http=" +
                (ts1.TotalMilliseconds / 1000.0).ToString("f2") + "s, parse=" +
                (ts2.TotalMilliseconds / 1000.0).ToString("f2") + "s, info2=" +
                (ts3.TotalMilliseconds / 1000.0).ToString("f2") + "s, db=" + 
                (ts4.TotalMilliseconds / 1000.0).ToString("f2") + "s");
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