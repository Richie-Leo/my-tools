using System;
using Pandora.Basis.Entity.Mapping;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 日K线数据
	/// </summary>
	[Table("k_japanese")]
	public partial class KJapaneseData
	{
		/// <summary>
		/// 实体属性->数据表字段 映射
		/// </summary>
		public static class Mapper
		{
			public const string TableName = "k_japanese";
			
			public const string Id = "id";
			public const string StockId = "sto_id";
			
			public const string TxDate = "tx_date";
			
			public const string PriceOpen = "p_open";
			public const string PriceMax = "p_max";
			public const string PriceMin = "p_min";
			public const string PriceClose = "p_close";
			public const string PricePrev = "p_prev";
			
			public const string Volume = "vol";
			public const string Amount = "amt";
			
			public const string MACusShort = "macs";
			public const string MACusLong = "macl";
			public const string VMACusShort = "vmacs";
			public const string VMACusLong = "vmacl";
			
			public const string MA5 = "ma5";
			public const string MA10 = "ma10";
			public const string MA20 = "ma20";
			public const string MA60 = "ma60";
			public const string MA120 = "ma120";
			public const string MA250 = "ma250";
			
			public const string PrevDate = "prev_date";
		}
		
		/// <summary>
		/// 无意义自增ID
		/// </summary>
		[Column(Name="id", Insertable=false, IsPrimary=true, IsSequence=true)]
		public int Id { get; set; }
		/// <summary>
		/// 证券ID
		/// </summary>
		[Column(Name="sto_id")]
		public int StockId { get; set; }
		/// <summary>
		/// 交易日期
		/// </summary>
		[Column(Name="tx_date")]
		public DateTime TxDate { get; set; }
		
		/// <summary>
		/// 开盘价
		/// </summary>
		[Column(Name="p_open")]
		public decimal PriceOpen { get; set; }
		/// <summary>
		/// 最高价
		/// </summary>
		[Column(Name="p_max")]
		public decimal PriceMax { get; set; }
		/// <summary>
		/// 最低价
		/// </summary>
		[Column(Name="p_min")]
		public decimal PriceMin { get; set; }
		/// <summary>
		/// 收盘价
		/// </summary>
		[Column(Name="p_close")]
		public decimal PriceClose { get; set; }
		/// <summary>
		/// 上一交易日收盘价
		/// </summary>
		[Column(Name="p_prev")]
		public decimal PricePrev { get; set; }
		
		/// <summary>
		/// 成交量（单位股）
		/// </summary>
		[Column(Name="vol")]
		public long Volume { get; set; }
		/// <summary>
		/// 成交额（单位元）
		/// </summary>
		[Column(Name="amt")]
		public long Amount { get;set; }
		
		/// <summary>
		/// 长期均价（定制算法）
		/// </summary>
		[Column(Name="macl")]
		public decimal MACusLong { get; set; }
		
		/// <summary>
		/// 短期均价（定制算法）
		/// </summary>
		[Column(Name="macs")]
		public decimal MACusShort { get; set; }
		
		/// <summary>
		/// 短期均量（定制算法）
		/// </summary>
		[Column(Name="vmacs")]
		public long VMACusShort { get; set; }	
		
		/// <summary>
		/// 长期均量（定制算法）
		/// </summary>
		[Column(Name="vmacl")]
		public long VMACusLong { get; set; }	
		
		/// <summary>
		/// 5日均线价（前5天均价）
		/// </summary>
		[Column(Name="ma5")]
		public decimal MA5 { get; set; }
		/// <summary>
		/// 10日均线价（前10天均价）
		/// </summary>
		[Column(Name="ma10")]
		public decimal MA10 { get; set; }
		/// <summary>
		/// 20日均线价（前20天均价）
		/// </summary>
		[Column(Name="ma20")]
		public decimal MA20 { get; set; }
		/// <summary>
		/// 60日均线价（前60天均价）
		/// </summary>
		[Column(Name="ma60")]
		public decimal MA60 { get; set; }
		/// <summary>
		/// 120日均线价（前120天均价）
		/// </summary>
		[Column(Name="ma120")]
		public decimal MA120 { get; set; }
		/// <summary>
		/// 250日均线价（前250天均价）
		/// </summary>
		[Column(Name="ma250")]
		public decimal MA250 { get; set; }
		
		/// <summary>
		/// 前一个交易日期
		/// </summary>
		[Column(Name="prev_date")]
		public DateTime PrevDate { get; set; }
	}
}
