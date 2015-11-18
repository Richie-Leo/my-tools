using System;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 策略选股执行结果。
	/// </summary>
	public partial class PickingResult
	{
		/// <summary>
		/// 实体属性->数据表字段 映射
		/// </summary>
		public static class Mapper
		{
			public const string TableName = "pick_result";

			public const string Id = "id";
			public const string PickId = "pick_id";
			public const string StockId = "sto_id";
			public const string StartDate = "s_date";
			public const string EndDate = "e_date";
			public const string TxDays = "tx_days";
			public const string VolDecrease = "vol_dec";
			public const string VolRatio = "vol_ratio";
			public const string VolTopDate = "vol_top_date";
			public const string MAShortInc = "mas_inc";
			public const string MAShortIncRatio = "mas_ratio";
			public const string MAShortTopDate = "mas_top_date";
			public const string MALongInc = "mal_inc";
			public const string MALongIncRatio = "mal_ratio";
			public const string MALongTopDate = "mal_top_date";
		}
		public int Id { get; set; }

		/// <summary>
		/// 选股操作ID。
		/// </summary>
		public int PickId { get; set; }

		/// <summary>
		/// 证券ID。
		/// </summary>
		public int StockId { get; set; }

		/// <summary>
		/// 匹配区间开始日期。
		/// </summary>
		public DateTime StartDate { get; set; }

		/// <summary>
		/// 匹配区间结束日期。
		/// </summary>
		public DateTime EndDate { get; set; }

		/// <summary>
		/// 匹配区间交易日数量。
		/// </summary>
		public int TxDays { get; set; }

		/// <summary>
		/// 是否缩量。
		/// </summary>
		public bool VolDecrease { get; set; }

		/// <summary>
		/// 缩量比例。
		/// </summary>
		public decimal VolRatio { get; set; }

		/// <summary>
		/// 前高日期。
		/// </summary>
		public DateTime VolTopDate { get; set; }

		/// <summary>
		/// 是否短期趋势上升。
		/// </summary>
		public bool MAShortInc { get; set; }

		/// <summary>
		/// 短期趋势涨幅。
		/// </summary>
		public decimal MAShortIncRatio { get; set; }

		/// <summary>
		/// 短期趋势顶点日期。
		/// </summary>
		public DateTime MAShortTopDate { get; set; }

		/// <summary>
		/// 是否长期趋势上升。
		/// </summary>
		public bool MALongInc { get; set; }

		/// <summary>
		/// 长期趋势涨幅。
		/// </summary>
		public decimal MALongIncRatio { get; set; }

		/// <summary>
		/// 长期趋势顶点日期。
		/// </summary>
		public DateTime MALongTopDate { get; set; }

		public PickingResult() {}
		private PickingResult(DataRow row) {
			this.Id = Convert.ToInt32(row[Mapper.Id]);
			this.PickId = Convert.ToInt32(row[Mapper.PickId]);
			this.StockId = Convert.ToInt32(row[Mapper.StockId]);
			this.StartDate = Convert.ToDateTime(row[Mapper.StartDate]);
			this.EndDate = Convert.ToDateTime(row[Mapper.EndDate]);
			this.TxDays = Convert.ToInt32(row[Mapper.TxDays]);
			this.VolDecrease = Convert.ToBoolean(row[Mapper.VolDecrease]);
			this.VolRatio = Convert.ToDecimal(row[Mapper.VolRatio]);
			this.VolTopDate = Convert.ToDateTime(row[Mapper.VolTopDate]);
			this.MAShortInc = Convert.ToBoolean(row[Mapper.MAShortInc]);
			this.MAShortIncRatio = Convert.ToDecimal(row[Mapper.MAShortIncRatio]);
			this.MAShortTopDate = Convert.ToDateTime(row[Mapper.MAShortTopDate]);
			this.MALongInc = Convert.ToBoolean(row[Mapper.MALongInc]);
			this.MALongIncRatio = Convert.ToDecimal(row[Mapper.MALongIncRatio]);
			this.MALongTopDate = Convert.ToDateTime(row[Mapper.MALongTopDate]);
		}

		public class PickingResultBulkInserter<T> : BulkInserter<T>{
			public PickingResultBulkInserter(Database db, int batchSize) : base(db, Mapper.TableName, new string[] {
					Mapper.PickId, Mapper.StockId, Mapper.StartDate, Mapper.EndDate, 
					Mapper.TxDays, Mapper.VolDecrease, Mapper.VolRatio, Mapper.VolTopDate, Mapper.MAShortInc, 
					Mapper.MAShortIncRatio, Mapper.MAShortTopDate, Mapper.MALongInc, Mapper.MALongIncRatio, Mapper.MALongTopDate
				}, batchSize) {}

			public override BulkInserter<T> Push(T obj){
				PickingResult e = obj as PickingResult;
				if(e == null) throw new EntityException("The type of obj is not PickingResult");
				base.Push(new object[] {
					e.PickId, e.StockId, e.StartDate, e.EndDate, 
					e.TxDays, e.VolDecrease, e.VolRatio, e.VolTopDate, e.MAShortInc, 
					e.MAShortIncRatio, e.MAShortTopDate, e.MALongInc, e.MALongIncRatio, e.MALongTopDate
				});
				return this;
			}
		}
	}
}