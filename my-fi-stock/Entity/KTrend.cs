using System;
using System.Data;
using System.Collections;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// K线量价趋势基类
	/// </summary>
	public class KTrend
	{
		public static class Mapper{
			public const string Id = "id";
			public const string StockId = "sto_id";
			public const string StartDate = "s_date";
			public const string EndDate = "e_date";
			public const string StartValue = "s_value";
			public const string EndValue = "e_value";
			public const string TxDays = "tx_days";
			public const string IncSpeed = "inc_speed";
		}
		
		public int Id { get; set; }
		/// <summary>
		/// 股票ID
		/// </summary>
		public int StockId { get; set; }
		/// <summary>
		/// 开始日期
		/// </summary>
		public DateTime StartDate { get; set; }
		/// <summary>
		/// 结束日期
		/// </summary>
		public DateTime EndDate { get; set; }
		/// <summary>
		/// 开始值
		/// </summary>
		public decimal StartValue { get; set; }
		/// <summary>
		/// 结束值
		/// </summary>
		public decimal EndValue { get; set; }
		/// <summary>
		/// 期间交易日个数
		/// </summary>
		public int TxDays { get; set; }
		private decimal _incSpeed;
		/// <summary>
		/// 增长速度
		/// </summary>
		public decimal IncSpeed { 
			get{
				return this._incSpeed;
			}
			set{
				this._incSpeed = Convert.ToDecimal(value.ToString("F4"));
			}
		}
		
		public KTrend() {}
		protected KTrend(DataRow row){
			this.Id = Convert.ToInt32(row[Mapper.Id]);
			this.StockId = Convert.ToInt32(row[Mapper.StockId]);
			this.StartDate = (DateTime)row[Mapper.StartDate];
			this.EndDate = (DateTime)row[Mapper.EndDate];
			this.StartValue = Convert.ToDecimal(row[Mapper.StartValue]);
			this.EndValue = Convert.ToDecimal(row[Mapper.EndValue]);
			this.TxDays = Convert.ToInt32(row[Mapper.TxDays]);
			this.IncSpeed = Convert.ToDecimal(row[Mapper.IncSpeed]);
		}
		
		protected static int BatchImport(Database db, string tableName, IList entities){
			if(entities == null || entities.Count<=0) return 0;
			int stockId = (entities[0] as KTrend).StockId;
			db.ExecNonQuery(string.Format("delete from {0} where {1}=?stoId", tableName, Mapper.StockId), 
			                new string[] { "stoId" }, new object[] { stockId });
			BulkInserter<KTrend> bi = CreateBulkInserter(db, tableName, 200); //初步验证，一批次插入200条性能最好
			for(int i=0; i<entities.Count; i++){
				bi.Push(entities[i] as KTrend);
			}
			bi.Flush();
			return entities.Count;
		}
		
		protected static DataSet FindAll(Database db, string tableName, int stockId){
			return db.ExecDataSet(
				string.Format("select * from " + tableName + " where {0}=?stoid order by {1}",
				             Mapper.StockId, Mapper.StartDate),
				new string[] { "stoid" },
				new object[] { stockId }
			);
		}
		
		private class KTrendBulkInserter<T> : BulkInserter<T>{
			public KTrendBulkInserter(Database db, string tableName, int batchSize) 
				: base(db, tableName, new string[] {
				       		Mapper.Id, Mapper.StockId, Mapper.StartDate, Mapper.EndDate,
				       		Mapper.StartValue, Mapper.EndValue, Mapper.TxDays, Mapper.IncSpeed
				       }, batchSize) {}
			
			public override BulkInserter<T> Push(T obj){
				KTrend e = obj as KTrend;
				if(e == null) throw new EntityException("The type of obj is not KTrend");
				base.Push(new object[] {
				    e.Id, e.StockId, e.StartDate, e.EndDate,
				    e.StartValue, e.EndValue, e.TxDays, e.IncSpeed
				});
				return this;
			}
		}
		
		public static BulkInserter<KTrend> CreateBulkInserter(Database db, string tableName, int batchSize){
			return new KTrendBulkInserter<KTrend>(db, tableName, batchSize);
	    }
	}
}