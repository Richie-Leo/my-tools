using System;
using System.Net;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Configuration;
using Winista.Text.HtmlParser;
using Winista.Text.HtmlParser.Lex;
using Winista.Text.HtmlParser.Util;
using Winista.Text.HtmlParser.Tags;
using Winista.Text.HtmlParser.Filters;
using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using System.Threading;

namespace Pandora.Invest.DataCapture.Http
{
	/// <summary>
	/// 从http://stock.finance.qq.com/corp1/stk_holder_count.php?zqdm=600111抓取股东数<br />
	/// 返回的html内容参考GetStockHolderCount_qq.html
	/// </summary>
	public static class ImpShareholdersNum
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ImpShareholdersNum));
		private const string PREFIX = "[http] [holder-num]> ";
		
		private const string URL = "http://stock.finance.qq.com/corp1/stk_holder_count.php?zqdm={0}";
		private const string ENCODING = "utf-8";
		private const int MAX_RETRIES = 3;
		
		private static void Info(string message){
			log.Info(PREFIX + message);
		}
		
		public static void DoGet(Database db, int startId){
			IList<Stock> stocks = Stock.FindAll(db);
			if(stocks.Count<=0) return;
			
			Info("开始抓取股东数数据，股票数量" + stocks.Count);
			int index = 0;
			bool debug = ConfigurationManager.AppSettings["debug"].ToLower() == "true" ? true : false;
			if(debug){
				if(!Directory.Exists("http-get"))
					Directory.CreateDirectory("http-get");
			}
			
			if(startId<=0) startId = 1;
			
			
			foreach(Stock stock in stocks){
				if(stock.StockId<startId) continue;
				DateTime start = DateTime.Now;
				
				int retries = 0;
				while(retries<MAX_RETRIES){
					retries++;
					string html = "";
					try{
						//抓取html
						HttpWebRequest req = (HttpWebRequest)WebRequest.Create(string.Format(URL, stock.StockCode));
						req.ContentType = "text/html";
						req.Method = "GET";
						HttpWebResponse resp = req.GetResponse() as HttpWebResponse;
						Stream stream = resp.GetResponseStream();
						StreamReader reader = new StreamReader(stream, Encoding.GetEncoding(ENCODING));
						html = reader.ReadToEnd();
						reader.Close();
						stream.Close();
						
						if(debug){
							string fileName = "http-get\\" + stock.StockCode + "-holder.html";
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
					Lexer lexer = new Lexer(html);
					Parser parser = new Parser(lexer);
					NodeList nodes = parser.Parse(new AndFilter(new NodeClassFilter(typeof(TableTag)), new HasAttributeFilter("CLASS")));
	    			INode table = FindHolderCountNode(nodes);
	    			if(table==null || table.Children==null || table.Children.Count<=0) continue;
	    			
	    			DateTime parse = DateTime.Now;
	    			
	    			int trIndex=-1;
	    			IList<ShareholdersNumEntity> entities = new List<ShareholdersNumEntity>();
	    			for(int i=0; i<table.Children.Count; i++){
	    				INode node = table.Children[i];
	    				if(!(node is ITag)) continue;
	    				ITag tag = node as ITag;
	    				if(tag.IsEndTag() || tag.TagName.Trim().ToLower()!="tr") continue;
	    				trIndex++;
	    				if(trIndex==0) continue; //第一行为标题行，忽略
	    				
	    				string s = GetTdText(tag, 0);
	    				DateTime date = DateTime.MinValue;
	    				DateTime.TryParse(s, out date);
	    				if(date<=DateTime.MinValue) continue;
	    				
	    				int holderCount = ConvertInt(GetTdText(tag, 1), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd")+", 无效股东数");
	    				int avgNum = ConvertInt(GetTdText(tag, 2), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd")+", 无效户均持股数");
	    				int totalShares = ConvertInt(GetTdText(tag, 3), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd")+", 无效总股本");
	    				int transShares = ConvertInt(GetTdText(tag, 4), "stock:" + stock.StockCode + ", date:" + date.ToString("yyyyMMdd")+", 无效流通股本");
	    				
	    				ShareholdersNumEntity entity = new ShareholdersNumEntity() {
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
	    			int insertedRows = ShareholdersNumEntity.Create(db, entities);
	    			
	    			TimeSpan ts1 = httpget - start, ts2 = parse - httpget, ts3 = DateTime.Now - parse;
	    			Info((index++) + " " + stock.StockCode + ": 网页抓取" + entities.Count + "，更新数据库" + insertedRows + "，耗时：http=" +
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
		
		/// <summary>
		/// 找table节点，且class="list list_d"
		/// </summary>
		/// <param name="nodes"></param>
		/// <returns></returns>
		private static INode FindHolderCountNode(NodeList nodes){
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
	}
}
