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
	public static class ImpShareholdersNum
	{
		public static void DoGet(Database db, int startId){
			IList<Stock> stocks = Stock.FindAll(db);
			if(stocks.Count<=0) return;

			if(startId<=0) startId = 1;
			
			
			foreach(Stock stock in stocks){

			}
		}
	}
}
