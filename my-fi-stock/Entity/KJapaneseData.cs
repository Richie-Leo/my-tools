using System;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 日K线。
	/// </summary>
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
		public int Id { get; set; }

		/// <summary>
		/// 证券ID。
		/// </summary>
		public int StockId { get; set; }

		/// <summary>
		/// 交易日期。
		/// </summary>
		public DateTime TxDate { get; set; }

		/// <summary>
		/// 开盘价。
		/// </summary>
		public decimal PriceOpen { get; set; }

		/// <summary>
		/// 最高价。
		/// </summary>
		public decimal PriceMax { get; set; }

		/// <summary>
		/// 最低价。
		/// </summary>
		public decimal PriceMin { get; set; }

		/// <summary>
		/// 收盘价。
		/// </summary>
		public decimal PriceClose { get; set; }

		/// <summary>
		/// 上一交易日价格。
		/// </summary>
		public decimal PricePrev { get; set; }

		/// <summary>
		/// 成交量（股）。
		/// </summary>
		public long Volume { get; set; }

		/// <summary>
		/// 成交额（元）。
		/// </summary>
		public long Amount { get; set; }

		/// <summary>
		/// 短期均价（定制算法）。
		/// </summary>
		public decimal MACusShort { get; set; }

		/// <summary>
		/// 长期均价（定制算法）。
		/// </summary>
		public decimal MACusLong { get; set; }

		/// <summary>
		/// 短期均量（定制算法）。
		/// </summary>
		public long VMACusShort { get; set; }

		/// <summary>
		/// 长期均量（定制算法）。
		/// </summary>
		public long VMACusLong { get; set; }

		/// <summary>
		/// 5日均线。
		/// </summary>
		public decimal MA5 { get; set; }

		/// <summary>
		/// 10日均线。
		/// </summary>
		public decimal MA10 { get; set; }

		/// <summary>
		/// 20日均线。
		/// </summary>
		public decimal MA20 { get; set; }

		/// <summary>
		/// 60日均线。
		/// </summary>
		public decimal MA60 { get; set; }

		/// <summary>
		/// 120日均线。
		/// </summary>
		public decimal MA120 { get; set; }

		/// <summary>
		/// 250日均线。
		/// </summary>
		public decimal MA250 { get; set; }

		/// <summary>
		/// 上一交易日日期。
		/// </summary>
		public DateTime PrevDate { get; set; }

		public KJapaneseData() {}
		private KJapaneseData(DataRow row) {
			this.Id = Convert.ToInt32(row[Mapper.Id]);
			this.StockId = Convert.ToInt32(row[Mapper.StockId]);
			this.TxDate = Convert.ToDateTime(row[Mapper.TxDate]);
			this.PriceOpen = Convert.ToDecimal(row[Mapper.PriceOpen]);
			this.PriceMax = Convert.ToDecimal(row[Mapper.PriceMax]);
			this.PriceMin = Convert.ToDecimal(row[Mapper.PriceMin]);
			this.PriceClose = Convert.ToDecimal(row[Mapper.PriceClose]);
			this.PricePrev = Convert.ToDecimal(row[Mapper.PricePrev]);
			this.Volume = Convert.ToInt64(row[Mapper.Volume]);
			this.Amount = Convert.ToInt64(row[Mapper.Amount]);
			this.MACusShort = Convert.ToDecimal(row[Mapper.MACusShort]);
			this.MACusLong = Convert.ToDecimal(row[Mapper.MACusLong]);
			this.VMACusShort = Convert.ToInt64(row[Mapper.VMACusShort]);
			this.VMACusLong = Convert.ToInt64(row[Mapper.VMACusLong]);
			this.MA5 = Convert.ToDecimal(row[Mapper.MA5]);
			this.MA10 = Convert.ToDecimal(row[Mapper.MA10]);
			this.MA20 = Convert.ToDecimal(row[Mapper.MA20]);
			this.MA60 = Convert.ToDecimal(row[Mapper.MA60]);
			this.MA120 = Convert.ToDecimal(row[Mapper.MA120]);
			this.MA250 = Convert.ToDecimal(row[Mapper.MA250]);
			this.PrevDate = Convert.ToDateTime(row[Mapper.PrevDate]);
		}

		public class KJapaneseDataBulkInserter<T> : BulkInserter<T>{
			public KJapaneseDataBulkInserter(Database db, int batchSize) : base(db, Mapper.TableName, new string[] {
					Mapper.StockId, Mapper.TxDate, Mapper.PriceOpen, Mapper.PriceMax, 
					Mapper.PriceMin, Mapper.PriceClose, Mapper.PricePrev, Mapper.Volume, Mapper.Amount, 
					Mapper.MACusShort, Mapper.MACusLong, Mapper.VMACusShort, Mapper.VMACusLong, Mapper.MA5, 
					Mapper.MA10, Mapper.MA20, Mapper.MA60, Mapper.MA120, Mapper.MA250, 
					Mapper.PrevDate
				}, batchSize) {}

			public override BulkInserter<T> Push(T obj){
				KJapaneseData e = obj as KJapaneseData;
				if(e == null) throw new EntityException("The type of obj is not KJapaneseData");
				base.Push(new object[] {
					e.StockId, e.TxDate, e.PriceOpen, e.PriceMax, 
					e.PriceMin, e.PriceClose, e.PricePrev, e.Volume, e.Amount, 
					e.MACusShort, e.MACusLong, e.VMACusShort, e.VMACusLong, e.MA5, 
					e.MA10, e.MA20, e.MA60, e.MA120, e.MA250, 
					e.PrevDate
				});
				return this;
			}
		}
	}
}