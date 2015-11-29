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
        #region 自动生成代码
        /// <summary>
        /// 实体属性->数据表字段 映射
        /// </summary>
        public static class Mapper
        {
            public const string Id = "id";
            public const string StockId = "sto_id";
            public const string StartDate = "s_date";
            public const string EndDate = "e_date";
            public const string StartValue = "s_value";
            public const string EndValue = "e_value";
            public const string TxDays = "tx_days";
            public const string ChangeSpeed = "chg_speed";
        }
        public int Id { get; set; }

        /// <summary>
        /// 证券ID。
        /// </summary>
        public int StockId { get; set; }

        /// <summary>
        /// 开始日期。
        /// </summary>
        public DateTime StartDate { get; set; }

        /// <summary>
        /// 结束日期。
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 开始值。
        /// </summary>
        public decimal StartValue { get; set; }

        /// <summary>
        /// 结束值。
        /// </summary>
        public decimal EndValue { get; set; }

        /// <summary>
        /// 交易日个数。
        /// </summary>
        public int TxDays { get; set; }
        #endregion

        private decimal _changeSpeed;
        /// <summary>
        /// 涨跌速度。例如：涨速为3.5%时，ChangeSpeed属性值为3.5，表示平均每个交易日上涨3.5%
        /// </summary>
        public decimal ChangeSpeed { 
            get{
                return this._changeSpeed;
            }
            set{
                this._changeSpeed = Convert.ToDecimal(value.ToString("F4"));
            }
        }

        #region 自动生成代码需经过稍微修改
        public KTrend() {}
        protected KTrend(DataRow row) {
            this.Id = Convert.ToInt32(row[Mapper.Id]);
            this.StockId = Convert.ToInt32(row[Mapper.StockId]);
            this.StartDate = Convert.ToDateTime(row[Mapper.StartDate]);
            this.EndDate = Convert.ToDateTime(row[Mapper.EndDate]);
            this.StartValue = Convert.ToDecimal(row[Mapper.StartValue]);
            this.EndValue = Convert.ToDecimal(row[Mapper.EndValue]);
            this.TxDays = Convert.ToInt32(row[Mapper.TxDays]);
            this.ChangeSpeed = Convert.ToDecimal(row[Mapper.ChangeSpeed]);
        }

        private class KTrendBulkInserter<T> : BulkInserter<T>{
            public KTrendBulkInserter(Database db, string tableName, int batchSize) 
                : base(db, tableName, new string[] {
                    Mapper.StockId, Mapper.StartDate, Mapper.EndDate,
                    Mapper.StartValue, Mapper.EndValue, Mapper.TxDays, Mapper.ChangeSpeed
                }, batchSize) {}

            public override BulkInserter<T> Push(T obj){
                KTrend e = obj as KTrend;
                if(e == null) throw new EntityException("The type of obj is not KTrend");
                base.Push(new object[] {
                    e.StockId, e.StartDate, e.EndDate,
                    e.StartValue, e.EndValue, e.TxDays, e.ChangeSpeed
                });
                return this;
            }
        }
        #endregion
		
        #region 非自动生成代码
        /// <summary>
        /// 计算涨跌速度，计算好的速度设置在ChangeSpeed属性上。
        /// 对下跌幅度计算方法进行了更改: 假设s为开始值，e为结束值，原本算法为 (e-s)/s*100%，修改后算法为 (s-e)/e*100%。
        /// 目的: 修正原本算法下涨幅、跌幅的不公平性，例如经历涨幅100%再经历跌幅50%，已经回到原价，这样相同的涨跌幅就不具备比较性
        /// </summary>
        /// <param name="k">K线数据</param>
        public static void CalChangeSpeed(KTrend k){
            if (k.TxDays <= 1 || k.StartValue==0)
                return;
            int dr = k.StartValue < k.EndValue ? 1 : -1;
            decimal lo = k.StartValue < k.EndValue ? k.StartValue : k.EndValue;
            decimal hi = k.EndValue > k.StartValue ? k.EndValue : k.StartValue;
            k.ChangeSpeed = Convert.ToDecimal(Math.Pow(Convert.ToDouble(hi / lo), Convert.ToDouble(1.0 / (k.TxDays - 1))) - 1) * 100 * dr;
        }

        /// <summary>
        /// 计算涨跌幅。
        /// 对下跌幅度计算方法进行了更改: 假设s为开始值，e为结束值，原本算法为 (e-s)/s*100%，修改后算法为 (s-e)/e*100%。
        /// 目的: 修正原本算法下涨幅、跌幅的不公平性，例如经历涨幅100%再经历跌幅50%，已经回到原价，这样相同的涨跌幅就不具备比较性
        /// </summary>
        /// <returns>The net change.</returns>
        /// <param name="s">S.</param>
        /// <param name="e">E.</param>
        public static decimal CalNetChange(decimal s, decimal e){
            if (s == 0 || s == e)
                return 0;
            if (e > s)
                return (e - s) / s;
            return (s - e) / e * -1;
        }

        /// <summary>
        /// 计算涨跌幅。
        /// 对下跌幅度计算方法进行了更改: 假设s为开始值，e为结束值，原本算法为 (e-s)/s*100%，修改后算法为 (s-e)/e*100%。
        /// 目的: 修正原本算法下涨幅、跌幅的不公平性，例如经历涨幅100%再经历跌幅50%，已经回到原价，这样相同的涨跌幅就不具备比较性
        /// </summary>
        /// <returns>The net change.</returns>
        /// <param name="s">S.</param>
        /// <param name="e">E.</param>
        public static decimal CalNetChange(long s, long e){
            if (s == 0 || s == e)
                return 0;
            if (e > s)
                return (e - s) * 1.0m / s;
            return (s - e) * 1.0m / e * -1;
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
		
		public static BulkInserter<KTrend> CreateBulkInserter(Database db, string tableName, int batchSize){
			return new KTrendBulkInserter<KTrend>(db, tableName, batchSize);
	    }
        #endregion
	}
}