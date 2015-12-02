using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// K线量价趋势基类
	/// </summary>
    public partial class KTrend
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
            public const string StartValue = "s_val";
            public const string EndDate = "e_date";
            public const string EndValue = "e_val";
            public const string HighValue = "hi_val";
            public const string LowValue = "lo_val";
            public const string TxDays = "tx_days";
            public const string NetChange = "net_chg";
            public const string ChangeSpeed = "chg_speed";
            public const string Amplitude = "am";
            public const string Remark = "remark";
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
        /// 开始值。
        /// </summary>
        public decimal StartValue { get; set; }

        /// <summary>
        /// 结束日期。
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 结束值。
        /// </summary>
        public decimal EndValue { get; set; }

        /// <summary>
        /// 最高值。
        /// </summary>
        public decimal HighValue { get; set; }

        /// <summary>
        /// 最低值。
        /// </summary>
        public decimal LowValue { get; set; }

        /// <summary>
        /// 交易日个数。
        /// </summary>
        public int TxDays { get; set; }
        #endregion

        #region 自动生成代码需经过稍微修改
        private string _remark;
        /// <summary>
        /// 备注。
        /// </summary>
        public string Remark
        {
            get { return this._remark; } 
            set{
                this._remark = value;
                if (!string.IsNullOrEmpty(this._remark) && this._remark.Length>200)
                    this._remark = this._remark.Substring(0, 200);
            } 
        }

        private decimal _netChange;
        /// <summary>
        /// 区间涨幅。
        /// </summary>
        public decimal NetChange { get { return this._netChange; } 
        	set { 
        		this._netChange = Convert.ToDecimal(value.ToString("F4"));
        		if(this._netChange>999) this._netChange = 999;        	
        	} 
        }

        private decimal _amplitude;
        /// <summary>
        /// 区间振幅。
        /// </summary>
        public decimal Amplitude { get {return this._amplitude;} 
        	set{
        		this._amplitude=Convert.ToDecimal(value.ToString("F4"));
        		if(this._amplitude>999)
        			this._amplitude = 999;
        	}
        }

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
                if(this._changeSpeed>999) this._changeSpeed = 999;
            }
        }

        public KTrend() {}
        protected KTrend(DataRow row) {
            this.Id = Convert.ToInt32(row[Mapper.Id]);
            this.StockId = Convert.ToInt32(row[Mapper.StockId]);
            this.StartDate = Convert.ToDateTime(row[Mapper.StartDate]);
            this.StartValue = Convert.ToDecimal(row[Mapper.StartValue]);
            this.EndDate = Convert.ToDateTime(row[Mapper.EndDate]);
            this.EndValue = Convert.ToDecimal(row[Mapper.EndValue]);
            this.HighValue = Convert.ToDecimal(row[Mapper.HighValue]);
            this.LowValue = Convert.ToDecimal(row[Mapper.LowValue]);
            this.TxDays = Convert.ToInt32(row[Mapper.TxDays]);
            this.NetChange = Convert.ToDecimal(row[Mapper.NetChange]);
            this.ChangeSpeed = Convert.ToDecimal(row[Mapper.ChangeSpeed]);
            this.Amplitude = Convert.ToDecimal(row[Mapper.Amplitude]);
            this.Remark = Convert.ToString(row[Mapper.Remark]);
        }

        public class KTrendBulkInserter<T> : BulkInserter<T>{
            public KTrendBulkInserter(Database db, string tableName, int batchSize) : base(db, tableName, new string[] {
                Mapper.StockId, Mapper.StartDate, Mapper.StartValue, Mapper.EndDate, 
                Mapper.EndValue, Mapper.HighValue, Mapper.LowValue, Mapper.TxDays, Mapper.NetChange, 
                Mapper.ChangeSpeed, Mapper.Amplitude, Mapper.Remark
            }, batchSize) {}

            public override BulkInserter<T> Push(T obj){
                KTrend e = obj as KTrend;
                if(e == null) throw new EntityException("The type of obj is not KTrend");
                base.Push(new object[] {
                    e.StockId, e.StartDate, e.StartValue, e.EndDate, 
                    e.EndValue, e.HighValue, e.LowValue, e.TxDays, e.NetChange, 
                    e.ChangeSpeed, e.Amplitude, e.Remark
                });
                return this;
            }
        }
        #endregion
	}
}