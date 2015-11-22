using System;
using System.Text;
using System.Collections.Generic;
using Pandora.Invest.Entity;
using Pandora.Basis.Utils;

namespace Pandora.Invest.PickingStrategy
{
	public abstract class AbstractPickingStrategy : IPickingStrategy
	{
		protected static log4net.ILog log = log4net.LogManager.GetLogger(typeof(AbstractPickingStrategy));
		
		/// <summary>
		/// 
		/// </summary>
		/// <param name="list"></param>
		/// <param name="date"></param>
		/// <param name="latestFirst"></param>
		/// <returns></returns>
		protected T FindMAData<T>(IList<T> list, DateTime date, bool latestFirst){
			if(list==null || list.Count<=0) return default(T);
			for(int i=list.Count-1; i>=0; i--){
				KTrend trend = list[i] as KTrend;
				if(date > trend.StartDate && date <= trend.EndDate) return list[i];
				if(date == trend.StartDate)
					return latestFirst ? list[i] : list[i==0 ? 0 : i-1];
			}
			return default(T);
		}
		
		protected KJapaneseData FindKData(IList<KJapaneseData> kdatas, DateTime date){
			foreach(KJapaneseData k in kdatas)
				if(k.TxDate == date) return k;
			return null;
		}
		
		public abstract IList<PickingResult> DoPicking(StrategyConfig conf, Stock sto, IList<KJapaneseData> kdatas
			, IList<KTrendMAShort> masList, IList<KTrendMALong> malList, IList<KTrendVMALong> vmalList);
	}
}