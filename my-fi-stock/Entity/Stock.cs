using System;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 股票基础属性。
	/// </summary>
	public partial class Stock
	{
		/// <summary>
		/// 实体属性->数据表字段 映射
		/// </summary>
		public static class Mapper
		{
			public const string TableName = "bas_stock";

			public const string StockId = "sto_id";
			public const string StockCode = "sto_code";
			public const string StockName = "sto_name";
			public const string ListDate = "list_date";
			public const string TotalCapital = "total_cap";
			public const string CirculatingCapital = "circ_cap";
			public const string EarningsPerShare = "eps";
			public const string NetAssetValuePerShare = "navps";
			public const string NetProfitGrowth = "profit_growth";
			public const string CompanyLocation = "com_loc";
			public const string Plate = "plate";
		}

		/// <summary>
		/// 证券ID。
		/// </summary>
		public int StockId { get; set; }

		/// <summary>
		/// 证券代码。
		/// </summary>
		public string StockCode { get; set; }

		/// <summary>
		/// 证券名称。
		/// </summary>
		public string StockName { get; set; }

		/// <summary>
		/// 上市日期。
		/// </summary>
		public DateTime ListDate { get; set; }

		/// <summary>
		/// 总股本（单位万股）。
		/// </summary>
		public long TotalCapital { get; set; }

		/// <summary>
		/// 流通股本（单位万股）。
		/// </summary>
		public long CirculatingCapital { get; set; }

		/// <summary>
		/// 每股净收益 Earnings per share（最近4季度动态每股净收益，由此计算得出动态市盈率）。
		/// </summary>
		public decimal EarningsPerShare { get; set; }

		/// <summary>
		/// 每股净资产 Net asset value per share。
		/// </summary>
		public decimal NetAssetValuePerShare { get; set; }

		/// <summary>
		/// 净利润增长率。
		/// </summary>
		public decimal NetProfitGrowth { get; set; }

		/// <summary>
		/// 所属地区。
		/// </summary>
		public string CompanyLocation { get; set; }

		/// <summary>
		/// 所属板块。
		/// </summary>
		public string Plate { get; set; }

		public Stock() {}
		private Stock(DataRow row) {
			this.StockId = Convert.ToInt32(row[Mapper.StockId]);
			this.StockCode = Convert.ToString(row[Mapper.StockCode]);
			this.StockName = Convert.ToString(row[Mapper.StockName]);
			this.ListDate = Convert.ToDateTime(row[Mapper.ListDate]);
			this.TotalCapital = Convert.ToInt64(row[Mapper.TotalCapital]);
			this.CirculatingCapital = Convert.ToInt64(row[Mapper.CirculatingCapital]);
			this.EarningsPerShare = Convert.ToDecimal(row[Mapper.EarningsPerShare]);
			this.NetAssetValuePerShare = Convert.ToDecimal(row[Mapper.NetAssetValuePerShare]);
			this.NetProfitGrowth = Convert.ToDecimal(row[Mapper.NetProfitGrowth]);
			this.CompanyLocation = Convert.ToString(row[Mapper.CompanyLocation]);
			this.Plate = Convert.ToString(row[Mapper.Plate]);
		}

		public class StockBulkInserter<T> : BulkInserter<T>{
			public StockBulkInserter(Database db, int batchSize) : base(db, Mapper.TableName, new string[] {
					Mapper.StockId, Mapper.StockCode, Mapper.StockName, Mapper.ListDate, Mapper.TotalCapital, 
					Mapper.CirculatingCapital, Mapper.EarningsPerShare, Mapper.NetAssetValuePerShare, Mapper.NetProfitGrowth, Mapper.CompanyLocation, 
					Mapper.Plate
				}, batchSize) {}

			public override BulkInserter<T> Push(T obj){
				Stock e = obj as Stock;
				if(e == null) throw new EntityException("The type of obj is not Stock");
				base.Push(new object[] {
					e.StockId, e.StockCode, e.StockName, e.ListDate, e.TotalCapital, 
					e.CirculatingCapital, e.EarningsPerShare, e.NetAssetValuePerShare, e.NetProfitGrowth, e.CompanyLocation, 
					e.Plate
				});
				return this;
			}
		}
	}
}