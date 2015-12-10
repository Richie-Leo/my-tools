using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using Pandora.Invest.MThread;

namespace Pandora.Invest.DataCapture
{
	/// <summary>
	/// 从文件导入日K线数据到数据库，文件格式通达信导出的excel格式文件。<br />
	/// 通达信版本：通达信可以批量导出日K线数据，每只股票一个文件。
	/// </summary>
	public class ImpKJapaneseWorker : MThreadWorker<string>{
		private Database _db;
		
		public override string LogPrefix {
			get {
				return "imp-kjap";
			}
		}
		
		public override void BeforeDo(MThreadContext context)
		{
			string connectionString = context.Get("connection-string").ToString();
			this._db = new Database(connectionString);
			this._db.Open();
		}
		
		public override void Do(MThreadContext context, string item)
		{
			List<KJapaneseData> kdatas = new List<KJapaneseData>();
			Stock stock = null;
			DateTime start = DateTime.Now;
			
            using(StreamReader sr = new StreamReader(item, Encoding.Default)){
				string line;
				int lineNum = 0;
				while((line = sr.ReadLine()) != null){
					lineNum++;
					string[] fields;
					
					//第一行：股票代码 股票名称 ...
					//600000 浦发银行 日线 前复权
					//000002 万  科Ａ 日线 前复权
					if(lineNum==1){ 
						fields = line.Split(' ');
						if(fields.Length<2){
							Info("Ignored. The first line is not in format: Code Name.");
							return;
						}
						string code = fields[0];
						int stockId = Convert.ToInt32(code); //股票代码必须是有效的数字
						if(stockId<=0){
							Info("Ignored. Invalidate code:" + code);
							return;
						}
						//有的股票名称中间有空格字符
						string name = line.Substring(code.Length, line.IndexOf("日线", StringComparison.CurrentCulture) - code.Length).Trim();
						stock = Stock.Get(_db, code);
						if(stock==null){ //股票在bas_stock中不存在，新建
                            stock = new Stock(){ StockId=stockId, StockCode=code, StockName=name };
                            stock.Create(_db);
                            return; 
						}
						continue;
					}
					
					//其余行 为日K线数据，格式：20100602,3.82,3.89,3.74,3.82,8709424,38275176.000
					//不符合该格式的行全部忽略
					if(string.IsNullOrEmpty(line) || line.Trim().Length<=0) continue;
					if(line[0]!='2' && line[0]!='1') continue;
					fields = line.Split(',');
					if(fields.Length<7) continue;
					if(fields[0].Trim().Length!=8) continue; //第一个字段为日期，格式yyyymmdd，不符合格式则忽略本行
					DateTime date = new DateTime(
						Convert.ToInt32(fields[0].Substring(0,4)),
						Convert.ToInt32(fields[0].Substring(4,2)),
						Convert.ToInt32(fields[0].Substring(6,2))
					);
					
					KJapaneseData e = new KJapaneseData(){
						    StockId = stock.StockId, TxDate = date,
						    OpenPrice = Convert.ToDecimal(fields[1]),
						    HighPrice = Convert.ToDecimal(fields[2]),
						    LowPrice = Convert.ToDecimal(fields[3]),
						    ClosePrice = Convert.ToDecimal(fields[4]),
						    Volume = Convert.ToInt64(Convert.ToDecimal(fields[5])),
						    Amount = Convert.ToInt64(Convert.ToDecimal(fields[6]))
					};
					if(e.ClosePrice>0.1m && e.OpenPrice>0.1m && e.Volume>0)
						kdatas.Add(e);
				}
			}
			
			DateTime loadFile = DateTime.Now;
			int importedRows = 0;
			if(kdatas.Count>0){	
				importedRows = KJapaneseData.BatchImport(_db, kdatas);
			}
			
			TimeSpan ts1 = loadFile - start, ts2 = DateTime.Now - loadFile, ts3 = DateTime.Now - start;
			Info("[" + stock.StockCode + "] Imported. Rows:" + importedRows + 
			     ", Elapsed:" + (ts3.TotalMilliseconds/1000.0).ToString("f1")
			     + "s, file:"+(ts1.TotalMilliseconds/1000.0).ToString("f1")
			     +"s, db:" + (ts2.TotalMilliseconds/1000.0).ToString("f1") + "s");
		}
		
		public override void AfterDo(MThreadContext context)
		{
			this._db.Close();
		}
	}
}