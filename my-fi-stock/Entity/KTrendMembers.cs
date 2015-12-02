using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
    public partial class KTrend{
        /// <summary>
        /// 计算涨跌速度，计算好的速度设置在ChangeSpeed属性上。
        /// 对下跌幅度计算方法进行了更改: 假设s为开始值，e为结束值，原本算法为 (e-s)/s*100%，修改后算法为 (s-e)/e*100%。
        /// 目的: 修正原本算法下涨幅、跌幅的不公平性，例如经历涨幅100%再经历跌幅50%，已经回到原价，这样相同的涨跌幅就不具备比较性
        /// </summary>
        /// <param name="k">K线数据</param>
        public static void CalChangeSpeed(KTrend trend){
            if (trend.TxDays <= 1 || trend.StartValue==0)
                return;
            int dr = trend.StartValue < trend.EndValue ? 1 : -1;
            decimal lo = trend.StartValue < trend.EndValue ? trend.StartValue : trend.EndValue;
            decimal hi = trend.EndValue > trend.StartValue ? trend.EndValue : trend.StartValue;
            trend.ChangeSpeed = Convert.ToDecimal(Math.Pow(Convert.ToDouble(hi / lo), Convert.ToDouble(1.0 / (trend.TxDays - 1))) - 1) * 100 * dr;
        }

        /// <summary>
        /// 计算区间最大值、最小值、振幅，计算好的结果直接设置在trend上
        /// </summary>
        /// <param name="trend">Trend.</param>
        /// <param name="lk">Lk.</param>
        /// <param name="start">Start.</param>
        /// <param name="end">End.</param>
        public static void CalHighLowValue(KTrend trend, MAType type, IList<KJapaneseData> lk, int start, int end){
            decimal hi = trend.StartValue, lo = trend.StartValue;
            for (int i = start + 1; i <= end; i++){
                decimal v = 0;
                switch (type){
                    case MAType.MAShort:
                    case MAType.MALong:
                        v = lk[i].ClosePrice;
                        break;
                    case MAType.VMAShort:
                    case MAType.VMALong:
                        v = lk[i].Volume;
                        break;
                }
                if (lk[i].ClosePrice > hi)
                    hi = lk[i].ClosePrice;
                if (lk[i].ClosePrice < lo)
                    lo = lk[i].ClosePrice;
            }
            trend.HighValue = hi;
            trend.LowValue = lo;
            trend.Amplitude = CalNetChange(lo, hi);
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
    }
}