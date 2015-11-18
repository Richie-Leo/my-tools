using System;
using System.IO;
using Pandora.Basis.DB;

namespace Pandora.Invest.DataCapture
{
	/// <summary>
	/// 从文件导入分时交易数据到数据库，文件格式为通达信导出的excel格式文件。<br />
	/// 通达信版本：分时交易数据只能每天导出一份文件。
	/// </summary>
	public static class ImportFileTimeShareTransaction
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(ImportFileTimeShareTransaction));
		private static string prefix = "[imp-trade]> ";
		
		private static void Info(string message){
			log.Info(prefix + message);
		}
		
		private static void Warn(string message){
			log.Warn(prefix + message);
		}
		
		public static void DoImportDirectory(Database db, string dir){
			if(string.IsNullOrEmpty(dir) || dir.Trim().Length<=0)
				return;
			string[] allFiles = Directory.GetFiles(dir);
			Info(">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>");
			Info("开始导入分时交易数据");
			Info("目录: " + dir + "，共" + allFiles.Length + "个文件");
			if(allFiles.Length<=0){
				Info("目录中没有有效分时交易数据文件");
				return;
			}
			DateTime start = DateTime.Now;
			for(int i=0; i<allFiles.Length; i++){
				Info( (i+1) + ". " + allFiles[i]);
				DoImportFile(db, allFiles[i]);
			}
			TimeSpan ts = DateTime.Now - start;
			Info("导入完成，共耗时" + (Convert.ToInt64(ts.TotalMilliseconds)/1000.0) + "秒");
		}
		
		public static void DoImportFile(Database db, string file){
//			DateTime start = DateTime.Now;
//			//分时交易数据的文件名格式：yyyymmdd-code.xls
//			//从文件名中读取日期、股票代码
//			string fileName = file.Substring(file.LastIndexOf('\\')+1); //去掉路径，取文件名
//			fileName = fileName.Substring(0, fileName.LastIndexOf('.'));  //去掉文件名后缀
//			string[] fields = fileName.Split('-'); //dateAndCode[0]: yyyymmdd，dateAndCode[1]: 6位股票代码
//			if(fields.Length!=2 || fields[0].Length!=8 || fields[1].Length!=6) {
//				Info("  文件名格式不符合规范，忽略该文件");
//				return;
//			}
//			string code = fields[1];
//			DateTime date = new DateTime(
//				int.Parse(fields[0].Substring(0, 4)), 
//				int.Parse(fields[0].Substring(4, 2)), 
//				int.Parse(fields[0].Substring(6, 2)));
//			
//			int exists = Convert.ToInt32(db.ExecScalar(
//				"select sto_id from bas_stock where sto_code=?code",
//				new string[]{"code"}, new object[]{code}
//			));
//			if(exists<=0){
//				Info("  股票" + code + "不存在，忽略该文件");
//				return;
//			}
//			int stockId = Convert.ToInt32(db.ExecScalar(
//				string.Format("select sto_id from bas_stock where sto_code='{0}'", code), null, null
//			));
//			
//			//如果已经存在日期为date的分时数据，先删除（可能在当天交易时间内导入过不完整的分时交易明细）
//			exists = Convert.ToInt32(db.ExecScalar(
//				string.Format("select count(*) from sto_trade_{0} where sto_id=?id and trade_time>?start and trade_time<?end", code),
//				new string[]{"id", "start", "end"}, new object[]{ stockId, date, date.AddDays(1) }
//			));
//			if(exists>0){
//				db.ExecNonQuery(string.Format("delete from sto_trade_{0} where sto_id=?id and trade_time>?start and trade_time<?end", code),
//				                new string[]{"id", "start", "end"}, new object[]{ stockId, date, date.AddDays(1) });
//			}
//
//			using(StreamReader sr = new StreamReader(file)){
//				BulkInsert bi = db.CreateBulkInsert(
//					string.Format("sto_trade_{0}", code), 
//					new string[] {"sto_id", "trade_time", "price", "qty", "match_num", "type"}, 
//					500
//				);
//				string line;
//				int lineNum = 0,  importedNum = 0;
//				while((line = sr.ReadLine()) != null){
//					lineNum++;
//					//前4行为无效数据，忽略空行
//					if(lineNum<5 || string.IsNullOrEmpty(line) || line.Trim().Length<=0) continue;
//					//交易时间为上午09:30-11:30，下午13:00-15:00，因此有效数据行以0或1开头
//					if(line[0]!='0' && line[0]!='1') continue;
//					//解析每一行
//					//0:time \t 1:price \t 2:qty \t 3:match_num \t 4:type
//					fields = line.Split('\t');
//					if(fields.Length<5) continue;
//					bi.PushValues(new object[] {
//				    	stockId, 
//				    	DateTime.Parse(date.ToString("yyyy-MM-dd") + " " + fields[0].Trim()),
//				    	Convert.ToDecimal(fields[1].Trim()),
//				    	Convert.ToInt32(fields[2].Trim()),
//				    	Convert.ToInt32(fields[3].Trim()),
//				    	fields[4].Trim().Length<=0 ? " " : fields[4].Trim()
//					});
//					importedNum++;
//				}
//				bi.Flush();
//				TimeSpan ts = DateTime.Now - start;
//				Info(code + "导入完成，导入" + importedNum + "条数据，耗时" + (Convert.ToInt64(ts.TotalMilliseconds)/1000.0) + "秒");
//			}
		}
	}
}