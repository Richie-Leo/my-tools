using System;
using System.Collections.Generic;
using System.Threading;

using Pandora.Invest.Entity;

namespace Pandora.Invest.MThread
{
	public abstract class MThreadWorker<T>
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MThreadWorker<T>));
		
		public abstract void BeforeDo(MThreadContext context);
		public abstract void Do(MThreadContext context, T item);
		public abstract void AfterDo(MThreadContext context);
		public abstract string LogPrefix {get;}
		
		private List<T> _items;
		private ProgressStatus _status;
		private MThreadContext _context;
		
		public List<T> Items { set{ this._items = value; } }
		public ProgressStatus Status { set{ this._status = value; } }
		public MThreadContext Context { set{ this._context = value; } }
        private T _item;
		
		public void Start(){
			DateTime start = DateTime.Now;
			int errorNum = 0;
			try{
				this.BeforeDo(this._context);
			}catch(Exception ex){
				if(this._status!=null){
					foreach(T item in this._items) this._status.FinishOne();
				}
				Error("BeforeDo Error:" + ex.Message, ex);
			}
			
			foreach(T obj in this._items){
                this._item = obj;
				try{
					this.Do(this._context, obj);
				}catch(Exception ex){
					errorNum++;
					Error("Task Error:" + ex.Message + ", item:" + obj.ToString(), ex);
				}
				if(this._status!=null) {
					this._status.FinishOne();
				}
                this._item = default(T);
			}
			
			try{
				this.AfterDo(this._context);
			}catch(Exception ex){
				Error("AfterDo Error:" + ex.Message, ex);
			}
			
			//记录日志
			TimeSpan ts = DateTime.Now - start;
			Info("Thread stopped, [" + start.ToString("mm:ss:fff") + "->" + DateTime.Now.ToString("mm:ss:fff")
			    + "], Total:" + this._items.Count + ", Error:" + errorNum + ", Elapsed:"
			    + (ts.TotalMilliseconds / 1000).ToString("F1") + "s");
		}
		
		protected void Info(string message){
            log.Info("[" + this.LogPrefix + "-" + Thread.CurrentThread.Name + "][" + DateTime.Now.ToString("mm:ss:fff") + "]"
                + (this._item == null ? "" : "[" + this._item.ToString() + "]") + "> " + message);
		}

        protected bool DebugEnabled { get { return log.IsDebugEnabled; } }
		
		protected void Debug(string message){
            log.Debug("[" + this.LogPrefix + "-" + Thread.CurrentThread.Name + "][" + DateTime.Now.ToString("mm:ss:fff") + "]"
                + (this._item == null ? "" : "[" + this._item.ToString() + "]") + "> " + message);
		}
		
		protected void Error(string message, Exception ex){
            log.Error("[" + this.LogPrefix + "-" + Thread.CurrentThread.Name + "][" + DateTime.Now.ToString("mm:ss:fff") + "]"
                + (this._item == null ? "" : "[" + this._item.ToString() + "]") + "> " + message, ex);
		}
	}
}