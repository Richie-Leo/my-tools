using System;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 策略选股执行日志。
	/// </summary>
	public partial class PickingLog
	{
		/// <summary>
		/// 实体属性->数据表字段 映射
		/// </summary>
		public static class Mapper
		{
			public const string TableName = "pick_log";

			public const string PickId = "pick_id";
			public const string Strategy = "strategy";
			public const string StartTime = "start_time";
			public const string EndTime = "end_time";
			public const string PickingMode = "pick_mode";
			public const string Params = "params";
		}

		/// <summary>
		/// 选股操作ID。
		/// </summary>
		public int PickId { get; set; }

		/// <summary>
		/// 所用策略。
		/// </summary>
		public string Strategy { get; set; }

		/// <summary>
		/// 操作开始时间。
		/// </summary>
		public DateTime StartTime { get; set; }

		/// <summary>
		/// 操作结束时间。
		/// </summary>
		public DateTime EndTime { get; set; }

		/// <summary>
		/// 选股模式。
		/// full在全量日K数据中匹配 latest仅在最新日K数据中匹配
		/// </summary>
		public string PickingMode { get; set; }

		/// <summary>
		/// 所用参数。
		/// </summary>
		public string Params { get; set; }

		public PickingLog() {}
		private PickingLog(DataRow row) {
			this.PickId = Convert.ToInt32(row[Mapper.PickId]);
			this.Strategy = Convert.ToString(row[Mapper.Strategy]);
			this.StartTime = Convert.ToDateTime(row[Mapper.StartTime]);
			this.EndTime = Convert.ToDateTime(row[Mapper.EndTime]);
			this.PickingMode = Convert.ToString(row[Mapper.PickingMode]);
			this.Params = Convert.ToString(row[Mapper.Params]);
		}

		public class PickingLogBulkInserter<T> : BulkInserter<T>{
			public PickingLogBulkInserter(Database db, int batchSize) : base(db, Mapper.TableName, new string[] {
					Mapper.Strategy, Mapper.StartTime, Mapper.EndTime, Mapper.PickingMode, 
					Mapper.Params
				}, batchSize) {}

			public override BulkInserter<T> Push(T obj){
				PickingLog e = obj as PickingLog;
				if(e == null) throw new EntityException("The type of obj is not PickingLog");
				base.Push(new object[] {
					e.Strategy, e.StartTime, e.EndTime, e.PickingMode, 
					e.Params
				});
				return this;
			}
		}
	}
}