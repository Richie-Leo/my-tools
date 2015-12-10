using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;
using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using System.Threading;
using Pandora.Basis.Utils;

namespace Pandora.Invest.DataCapture.Http
{
	/// <summary>
	/// Description of StockBasicInfo.
	/// </summary>
	public class ImpStockBasicInfo
	{
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ImpStockBasicInfo));
		private const string PREFIX = "[http] [basic-info] [";
		
		private const string URL = "http://gu.qq.com/{0}{1}";
		private const string ENCODING = "utf-8";
		private const int MAX_RETRIES = 3;
		private const int THREAD_NUM = 6;
		private static bool _debug = false;
		
		private static void Info(string message){
			log.Info(PREFIX + Thread.CurrentThread.Name + "]> " + message);
		}
		
		public static void DoGet(string connectionString, ProgressStatus status){
			Database db = new Database(connectionString);
			db.Open();
			IList<Stock> stocks = Stock.FindAll(db);
			db.Close();
			if(stocks.Count<=0) return;
			
			Info("开始抓取股票基础信息，股票数量" + stocks.Count);
			_debug = ConfigurationManager.AppSettings["debug"].ToLower() == "true" ? true : false;
			if(_debug){
				if(!Directory.Exists("http-get"))
					Directory.CreateDirectory("http-get");
			}

			List<List<Stock>> blocks = new List<List<Stock>>(THREAD_NUM);
			for(int i=0; i<THREAD_NUM; i++)
				blocks.Add(new List<Stock>());
			for(int i=0; i<stocks.Count; i++){
				blocks[i % THREAD_NUM].Add(stocks[i]);
			}
			status.Start(stocks.Count);
			for(int i=0; i<THREAD_NUM; i++){
				Thread t = new Thread(new ImpBasicInfoWorker(connectionString, blocks[i], status).DoGet);
				t.Name = "thread-" + i.ToString("00");
				t.IsBackground = true;
				t.Priority = ThreadPriority.AboveNormal;
				t.Start();
			}
		}
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="trIndex">从0开始的位置索引</param>
		/// <returns></returns>
		private static ITag GetTr(INode parent, int trIndex){
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
		private static string GetTdText(ITag parent, int tdIndex){
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
		
		private static int ConvertInt(string str, string message){
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
		
		public class ImpBasicInfoWorker{
			private string _connectionString;
			private IList<Stock> _stocks;
			private ProgressStatus _status;
			public ImpBasicInfoWorker(string connectionString, IList<Stock> stocks, ProgressStatus status){
				this._connectionString=connectionString;
				this._stocks=stocks;
				this._status = status;
			}
			
			public void DoGet(){
				Database db = new Database(this._connectionString);
				db.Open();
				foreach(Stock stock in this._stocks) {
					GetInfo(db, stock);
					this._status.FinishOne();
				}
				db.Close();
			}
			
			private void GetInfo(Database db, Stock stock){
				DateTime start = DateTime.Now;
				
				int retries = 0;
				while(retries<MAX_RETRIES){
					retries++;
					string html = "";
					try{
						//抓取html
						HttpWebRequest req = (HttpWebRequest)WebRequest.Create(
							string.Format(URL, stock.StockCode[0]=='6'?"sh":"sz", stock.StockCode)
						);
						req.ContentType = "text/html";
						req.Method = "GET";
						HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
						Stream stream = resp.GetResponseStream();
						StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(ENCODING));
						html = reader.ReadToEnd();
						reader.Close();
						stream.Close();
						
						if(_debug){
							string fileName = "http-get\\" + stock.StockCode + "-bas-info.html";
							if(File.Exists(fileName)) File.Delete(fileName);
							File.CreateText(fileName).Close();
							File.WriteAllText(fileName, html, Encoding.GetEncoding(ENCODING));
						}
					}catch(Exception ex){
						Info(stock.StockCode + ": ERROR: HTTP错误，" + ex.Message);
						Thread.Sleep(1 * 1000);
						continue;
					}
					
					DateTime httpget = DateTime.Now;
					
					//解析html
					int p1 = html.IndexOf("<table class=\"data\">");
					if(p1 < 0) {
						Info(stock.StockCode + ": 返回的HTML格式错误，未发现 公司概况 节点：<table class=\"data\">");
						break;
					}
					int p2 = html.IndexOf("</table>", p1);
					string tableStr = html.Substring(p1, p2 - p1 + "</table>".Length);
					
					Lexer lexer = new Lexer(tableStr);
					Parser parser = new Parser(lexer);
					NodeList nodes = parser.Parse(null);
					if(nodes==null || nodes.Count<=0) {
						Info(stock.StockCode + ": HTMLParser未解析出table节点");
						break;
					}
	    			
	    			DateTime parse = DateTime.Now;
	    			
	    			string text, value;
	    			decimal dec;
	    			INode table = nodes[0];
	    			
	    			ITag tr = GetTr(table, 0); //第1行
	    			//所属地区
	    			stock.CompanyLocation = GetTdText(tr, 1);
	    			//总股本
	    			text = GetTdText(tr, 2);
	    			value = GetTdText(tr, 3);
	    			dec = ConvertUtil.ToDecimal(value, 0);
	    			if(text.IndexOf('亿')>=0) stock.TotalCapital = Convert.ToInt64(dec * 100000000);
	    			else if(text.IndexOf('万')>=0) stock.TotalCapital = Convert.ToInt64(dec * 10000);
	    			else stock.TotalCapital = Convert.ToInt64(dec);
	    			
	    			tr = GetTr(table, 1); //第2行
	    			//上市时间
	    			stock.ListDate = ConvertUtil.ToDateTime(GetTdText(tr, 1), stock.ListDate);
	    			//流通股本
	    			text = GetTdText(tr, 2);
	    			value = GetTdText(tr, 3);
	    			dec = ConvertUtil.ToDecimal(value, 0);
	    			if(text.IndexOf('亿')>=0) stock.CirculatingCapital = Convert.ToInt64(dec * 100000000);
	    			else if(text.IndexOf('万')>=0) stock.CirculatingCapital = Convert.ToInt64(dec * 10000);
	    			else stock.CirculatingCapital = Convert.ToInt64(dec);
	    			//每股净资产
	    			stock.NetAssetValuePerShare = ConvertUtil.ToDecimal(GetTdText(tr, 5), stock.NetAssetValuePerShare);
	    			
	    			tr = GetTr(table, 3); //第4行
	    			stock.NetProfitGrowth = ConvertUtil.ToDecimal(GetTdText(tr, 1), stock.NetProfitGrowth);
	    			
	    			stock.UpdateInfo(db);
	    			
	    			TimeSpan ts1 = httpget - start, ts2 = parse - httpget, ts3 = DateTime.Now - parse;
	    			Info(stock.StockCode + ": 耗时：http=" +
	    			    (Convert.ToInt64(ts1.TotalMilliseconds)/1000.0) + "s, parse=" + 
	    			    (Convert.ToInt64(ts2.TotalMilliseconds)/1000.0) + "s, db=" + 
	    			    (Convert.ToInt64(ts3.TotalMilliseconds)/1000.0));
	    			break;
				}
				if(retries>=MAX_RETRIES){
					Info(stock.StockCode + ": 3次错误，放弃抓取");
				}
			}
		}
	}
}