using System;

namespace Pandora.Invest
{
	public class ProgressStatus
	{
		private enum State { Uninitialized, Running, Stopped }
		
		private State _state = State.Uninitialized;
		
		/// <summary>
		/// 任务总数
		/// </summary>
		public int TotalNum = 0;
		/// <summary>
		/// 已完成数
		/// </summary>
		public int FinishedNum = 0;
		/// <summary>
		/// 开始时间
		/// </summary>
		public DateTime StartTime;
		/// <summary>
		/// 完成时间
		/// </summary>
		public DateTime EndTime;
		
		/// <summary>
		/// 是否已经结束
		/// </summary>
		/// <returns></returns>
		public bool HasFinished { 
			get{
				return this._state==State.Stopped;
			}
		}
		
		public bool IsRunning {
			get{
				return this._state == State.Running;
			}
		}
		
		/// <summary>
		/// 当前进度百分比（进度为76.5%时返回值为76.5）
		/// </summary>
		/// <returns></returns>
		public decimal CurrentRate {
			get{
				if(this.TotalNum==0) return 0M;
				return this.FinishedNum*100.0M/this.TotalNum;
			}
		}
		
		/// <summary>
		/// 已用时（秒）
		/// </summary>
		/// <returns></returns>
		public decimal ElapsedSeconds {
			get{
				TimeSpan ts = new TimeSpan(0);
				if(this.HasFinished)
					ts = this.EndTime - this.StartTime;
				else
					ts = DateTime.Now - this.StartTime;
				return Convert.ToDecimal(ts.TotalMilliseconds) / 1000;
			}
		}
		
		/// <summary>
		/// 剩余待处理数量
		/// </summary>
		public int RemainingNum {
			get{
				return this.TotalNum - this.FinishedNum;
			}
		}
		
		/// <summary>
		/// 完成了1个操作，更新进度状态
		/// </summary>
		public void FinishOne(){
			this.FinishedNum++;
			if(this.FinishedNum>=this.TotalNum){
				this.EndTime = DateTime.Now;
				this._state = State.Stopped;
			}
		}
		
		public void Start(int totalNum){
			if(this.IsRunning) return;
			this.TotalNum = totalNum;
			this.FinishedNum = 0;
			this.StartTime = DateTime.Now;
			this.EndTime = DateTime.MinValue;
			this._state = State.Running;
		}
	}
}
