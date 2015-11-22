using System;
using System.Collections.Generic;

using Pandora.Basis.DB;
using Pandora.Invest.MThread;
using Pandora.Invest.Entity;

namespace Pandora.Invest.PickingStrategy
{
	/// <summary>
	/// 选股策略的多线程执行者。
	/// </summary>
	public class PickingStrategyWorker : MThreadWorker<Stock>
	{
		private const string PICKING_LOG = "==picking-log==";
		private const string RUNNING_THREADS = "==running-threads==";
		private static object _lockObj = new object();
		
		private IList<IPickingStrategy> _strategies = null;
		private Database _db;
		private PickingResult.PickingResultBulkInserter<PickingResult> _bi;
		private IList<StrategyConfig> _configs;
		
		public PickingStrategyWorker() {}
			
		public override string LogPrefix {
			get {
					return "picking";
			}
		}
			
		public override void BeforeDo(MThreadContext context)
		{
			//创建策略执行者清单
			this._configs = context.Get("executors") as IList<StrategyConfig>;
			if(this._configs==null || this._configs.Count<=0) return;
			this._strategies = new List<IPickingStrategy>(this._configs.Count);
			foreach(StrategyConfig conf in this._configs){
				string typeName = conf.StrategyClass;
				if(typeName.IndexOf('.')<0) typeName = "Pandora.Invest.PickingStrategy." + typeName;
				Type type = Type.GetType(typeName);
				IPickingStrategy strategy = Activator.CreateInstance(type) as IPickingStrategy;
				this._strategies.Add(strategy);
			}
			
			//初始化数据库对象
			string connectionString = context.Get("connection-string").ToString();
			this._db = new Database(connectionString);
			this._db.Open();
			//创建批量插入器
			this._bi = new PickingResult.PickingResultBulkInserter<PickingResult>(this._db, 200);
			
			//设置运行中线程计数器
			lock(_lockObj){
				if(!context.Contains(RUNNING_THREADS))
					context.Put(RUNNING_THREADS, 0);
				int runningThreads = Convert.ToInt32(context.Get(RUNNING_THREADS));
				runningThreads++;
				context.Put(RUNNING_THREADS, runningThreads);
			}
			
			//为每个策略创建选股执行日志，并放入context中供选股期间使用
			//Worker会多实例多线程运行，而创建过程只能执行1次，因此使用context作为锁进行线程同步
			if(!context.Contains(PICKING_LOG)){
				lock(_lockObj){
					if(!context.Contains(PICKING_LOG)){
						IList<PickingLog> pickingLogs = new List<PickingLog>(_strategies.Count);
						for(int i=0; i<this._strategies.Count; i++){
							PickingLog pLog = new PickingLog(){ 
								Strategy = "",
								PickingMode = "", //TODO: 去掉PickingMode
								StartTime = DateTime.Now,
								EndTime = DateTime.MinValue,
								Params = _configs[i].ConfigString
							};
							pLog.Create(this._db);
							pickingLogs.Add(pLog);
						}
						context.Put(PICKING_LOG, pickingLogs);
					}
				}
			}
		}
		
		public override void Do(MThreadContext context, Stock item)
		{
			if(item.IsSTStock()){
				Debug("{" + item.StockCode + "-" + item.StockName + "} {ignored}: ST");
				return;
			}
			IList<KJapaneseData> kdatas = KJapaneseData.FindLatest(_db, item.StockId, 750);
			if(kdatas.Count<=0) {
				Debug("{" + item.StockCode + "-" + item.StockName + "} {ignored}: k-data not found");
				return;
			}
			IList<KTrendMAShort> masList = KTrendMAShort.FindAll(_db, item.StockId);
			IList<KTrendMALong> malList = KTrendMALong.FindAll(_db, item.StockId);
			IList<KTrendVMALong> vmalList = KTrendVMALong.FindAll(_db, item.StockId);
			
			IList<PickingLog> pickingLogs = (IList<PickingLog>)context.Get(PICKING_LOG);
			for(int i=0; i<this._strategies.Count; i++){
				var matchedList = _strategies[i].DoPicking(_configs[i], item, kdatas, masList, malList, vmalList);
				for(int j=0; matchedList!=null && j<matchedList.Count; j++){
					matchedList[j].PickId = pickingLogs[i].PickId;
					matchedList[j].StockId = item.StockId;
					this._bi.Push(matchedList[j]);
					Info("{" + item.StockCode + "-" + item.StockName + "} {matched}: "
					     + matchedList[j].StartDate.ToString("yyyyMMdd") + "->"
					     + matchedList[j].EndDate.ToString("yyyyMMdd") + "(" + matchedList[j].TxDays + ")");
				}
			}
		}
		
		public override void AfterDo(MThreadContext context)
		{
			this._bi.Flush();
			//恢复运行中线程计数器
			int runningThreads = 0;
			lock(_lockObj){
				runningThreads = Convert.ToInt32(context.Get(RUNNING_THREADS));
				runningThreads--;
				context.Put(RUNNING_THREADS, runningThreads);
			}
			//更新选股策略执行日志的结束时间
			if(runningThreads<=0){
				try{
					if(context.Contains(PICKING_LOG)){
						lock(_lockObj){
							if(context.Contains(PICKING_LOG)){
								IList<PickingLog> pickingLogs = (IList<PickingLog>)context.Get(PICKING_LOG);
								for(int i=0; i<pickingLogs.Count; i++){
									pickingLogs[i].EndTime = DateTime.Now;
									pickingLogs[i].UpdateEndTime(this._db);
								}
								context.Remove(PICKING_LOG);
							}
						}
					}
				}catch(Exception ex){
					Info("Error on updating EndTime of PickingLogs: " + ex.Message);
				}
			}
			this._db.Close();
		}
	}
}