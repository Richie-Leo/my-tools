using System;
using System.Collections.Generic;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 股东数
	/// </summary>
	public class ShareholdersNumEntity
	{
		private const string INSERT_SQL = "insert into sto_holder_num" +
			" (sto_id, report_date, holders_num, var_num, avg_shares, total_shares, trans_shares, create_time, source) " +
			" values(?id, ?date, ?holdersNum, ?varNum, ?avgShares, ?totalShares, ?transShares, ?time, ?source)";
		
		#region Properties Definition
		/// <summary>
		/// 股票ID
		/// </summary>
		public int StockId { get; set; }
		/// <summary>
		/// 披露日期
		/// </summary>
		public DateTime ReportDate { get; set; }
		/// <summary>
		/// 股东数
		/// </summary>
		public int HoldersNum { get; set; }
		/// <summary>
		/// 与上期相比股东数增减量
		/// </summary>
		public int VarNum { get; set; }
		/// <summary>
		/// 户均持股数
		/// </summary>
		public int AvgShares { get; set; }
		/// <summary>
		/// 总股本数
		/// </summary>
		public int TotalShares { get; set; }
		/// <summary>
		/// 流通股数量
		/// </summary>
		public int TransShares { get; set; }
		/// <summary>
		/// 数据来源处
		/// </summary>
		public string Source { get; set; }
		/// <summary>
		/// 添加时间
		/// </summary>
		public DateTime CreateTime { get; set; }
		#endregion
	
		public static IList<ShareholdersNumEntity> FindLatest(Database db, DateTime startDate){
			DataSet ds = db.ExecDataSet(
				"select * from sto_holder_num where report_date>=?date order by sto_id asc, report_date desc",
				new string[]{"date"}, new object[]{startDate}
			);
			IList<ShareholdersNumEntity> result = new List<ShareholdersNumEntity>();
			if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
				foreach(DataRow row in ds.Tables[0].Rows){
					result.Add(BuildEntity(row));
				}
			}
			return result;
		}
		
		private static ShareholdersNumEntity BuildEntity(DataRow row){
			return new ShareholdersNumEntity(){
				StockId = Convert.ToInt32(row["sto_id"]),
				ReportDate = (DateTime)row["report_date"],
				HoldersNum = (int)row["holders_num"],
				VarNum = (int)row["var_num"],
				AvgShares = (int)row["avg_shares"],
				TotalShares = (int)row["total_shares"],
				TransShares = (int)row["trans_shares"],
				CreateTime = (DateTime)row["create_time"],
				Source = row["source"].ToString()
			};
		}
		
		/// <summary>
		/// 创建股东数数据。前提条件：<br />
		/// 1. 每次调用，<paramref name="entities"/>只能包含同一只股票的股东数数据；<br />
		/// 2. 调用时，必须确保StockId, PublishDate, HolderCount, AverageStockNumber, Source属性值有效；
		/// </summary>
		/// <param name="db"></param>
		/// <param name="entities"></param>
		public static int Create(Database db, IList<ShareholdersNumEntity> entities){
			if(entities==null || entities.Count<=0) return 0;
			
			DateTime minDate = DateTime.MaxValue, effectiveDate = new DateTime(1990, 1, 1);
			int stockId = 0, exists=0, insertedRows = 0;
			try{
				db.BeginTransaction();
				//添加数据
				foreach(ShareholdersNumEntity entity in entities){
					//数据校验
					if(entity.StockId<=0 || entity.ReportDate<=effectiveDate 
					   // || entity.HolderCount<=0 || entity.AverageStockNumber<=0
					   || (entity.Source==null || entity.Source.Trim().Length<=0))
						throw new EntityException("[holder-num] [create] 属性无效，无法更新数据库，[id:" 
						                          + entity.StockId + ", date:" + entity.ReportDate.ToString("yyyyMMdd"));
					if(stockId==0) stockId = entity.StockId;
					if(stockId!=entity.StockId)
						throw new EntityException("[holder-num] [create] entities中包含了多只股票的股东数数据");
					
					entity.CreateTime = DateTime.Now;
					entity.VarNum = 0;
					//插入数据
					exists = Convert.ToInt32(db.ExecScalar(
						"select count(*) from sto_holder_num where sto_id=?id and report_date=?date", 
						new string[] {"id", "date"},
						new object[] { entity.StockId, entity.ReportDate}
					));
					if(exists<=0){
						insertedRows += db.ExecNonQuery(INSERT_SQL, 
			                new string[]{"id", "date", "holdersNum", "varNum", "avgShares", "totalShares", "transShares", "time", "source"}, 
			                new object[]{entity.StockId, entity.ReportDate, entity.HoldersNum, entity.VarNum
			                		, entity.AvgShares, entity.TotalShares, entity.TransShares
			                		, entity.CreateTime, entity.Source});
						
						if(minDate>entity.ReportDate) minDate = entity.ReportDate;
					}
				}
				
				//更新股东数增长量
				int prevCount = 0;
				exists = Convert.ToInt32(db.ExecScalar(
					"select count(*) from sto_holder_num where sto_id=?id and report_date<?date", 
					new string[] {"id", "date"},
					new object[] { stockId, minDate}
				));
				if(exists>0){
					prevCount = Convert.ToInt32(db.ExecScalar(
						"select holders_num from sto_holder_num where sto_id=?id and report_date<?date order by report_date desc limit 1", 
						new string[] {"id", "date"},
						new object[] { stockId, minDate}
					));
				}
				DataSet ds = db.ExecDataSet(
					"select * from sto_holder_num where sto_id=?id and report_date>=?date order by report_date asc",
					new string[] { "id", "date" }, new object[] { stockId, minDate }
				);
				foreach(DataRow row in ds.Tables[0].Rows){
					if(prevCount==0){
						prevCount = Convert.ToInt32(row["holders_num"]);
						continue;
					}
					int curCount = Convert.ToInt32(row["holders_num"]);
					db.ExecNonQuery(
						"update sto_holder_num set var_num=?varNum where sto_id=?id and report_date=?date", 
						new string[] { "id", "date", "varNum" },
						new object[] { stockId, row["report_date"],  curCount-prevCount}
					);
					prevCount = curCount;
				}
				db.CommitTransaction();
			}catch(Exception ex){
				db.RollbackTransaction();
				throw ex;
			}
			
			return insertedRows;
		}
	}
}
