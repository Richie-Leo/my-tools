using System;

namespace Pandora.Invest.Entity
{
	public partial class Stock
	{
		public static class Mapper
		{
			public const string TableName = "bas_stock";
			
			public const string Id = "sto_id";
			public const string Code = "sto_code";
			public const string Name = "sto_name";

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
		/// ID
		/// </summary>
		public int StockId { get; set; }
		/// <summary>
		/// 代码
		/// </summary>
		public string StockCode { get; set; }
		/// <summary>
		/// 名称
		/// </summary>
		public string StockName { get; set; }
		/// <summary>
		/// 上市日期
		/// </summary>
		public DateTime ListDate { get; set; }
		/// <summary>
		/// 总股本（单位万股）
		/// </summary>
		public long TotalCapital { get; set; }
		/// <summary>
		/// 流通股本（单位万股）
		/// </summary>
		public long CirculatingCapital { get; set; }
		/// <summary>
		/// 每股收益
		/// </summary>
		public decimal EarningsPerShare { get; set; }
		/// <summary>
		/// 每股净资产
		/// </summary>
		public decimal NetAssetValuePerShare { get; set; }
		/// <summary>
		/// 净利润增长率（增长率为12.61%，则属性值为12.61）
		/// </summary>
		public decimal NetProfitGrowth { get; set; }
		/// <summary>
		/// 公司所属地区
		/// </summary>
		public string CompanyLocation { get; set; }
		/// <summary>
		/// 所属板块
		/// </summary>
		public string Plate { get; set; }
	}
}