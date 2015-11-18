using System;
using System.Collections.Generic;
using System.Threading;

namespace Pandora.Invest.MThread
{
	public class MThreadManager<T>
	{
		private int _threadNum = 0;
		private Type _workClass;
		private ProgressStatus _status;
		private List<T> _todoItems = new List<T>();
		private MThreadContext _context = new MThreadContext();
		
		public MThreadManager(int threadNum, Type workClass, ProgressStatus status)
		{
			this._threadNum = threadNum;
			this._workClass = workClass;
			this._status = status;
		}
		
		public MThreadManager<T> SetContext(string name, object value){
			this._context.Put(name, value);
			return this;
		}
		
		public MThreadManager<T> AddItem(T item){
			this._todoItems.Add(item);
			return this;
		}
		
		public MThreadManager<T> AddItem(ICollection<T> items){
			if(items!=null){
				foreach(T obj in items)
					this._todoItems.Add(obj);
			}
			return this;
		}
		
		public void Start(){
			this._status.Start(this._todoItems.Count);
			
			List<T>[] blocks = new List<T>[this._threadNum];
			for(int i=0; i<this._threadNum; i++)
				blocks[i] = new List<T>();
			for(int i=0; i<this._todoItems.Count; i++)
				blocks[i % this._threadNum].Add(this._todoItems[i]);
			for(int i=0; i<this._threadNum; i++){
				MThreadWorker<T> worker = (MThreadWorker<T>)Activator.CreateInstance(_workClass);
				worker.Items = blocks[i];
				worker.Status = this._status;
				worker.Context = this._context;
				
				Thread thread = new Thread(worker.Start);
				thread.IsBackground = true;
				thread.Name = "t" + i.ToString("00");
				thread.Priority = ThreadPriority.Normal;
				
				thread.Start();
			}
		}
	}
}