//using System;
//using System.Collections.Generic;
//using System.Collections;
//using Pandora.Invest.Entity;
//
//namespace Pandora.Invest.PickingStrategy
//{
//	/// <summary>
//	/// 地量检测
//	/// </summary>
//	public class MinVolumeStrategy : AbstractPickingStrategy
//	{
//		private const int SCAN_RANGE_IN_MONTHS = 24; //扫描12个月
//		private const int MATCH_RANGE_IN_DAYS = 10; //在最近10个交易日进行匹配
//		private const int TOP_N_MIN = 10;
//		private const int IGNORE_THRESHOLD = 60; //新股，交易日少于该数目则忽略
//		
//		public MinVolumeStrategy() : base(null, null) {}
//		
//		public override string TypeName { get { return "MinVol"; } }
//		
//		/// <summary>
//		/// 
//		/// </summary>
//		/// <param name="stock"></param>
//		/// <param name="klines">必须按照交易日升序排列</param>
//		/// <param name="sampleDate"></param>
//		/// <returns></returns>
//		public override PickingResult DoFilter(Stock stock, List<KJapaneseData> klines, DateTime sampleDate){
//			PickingResult result = new PickingResult() { Matched=false };
//			
//			DateTime sinceDate = DateTime.Now.AddMonths(-1 * SCAN_RANGE_IN_MONTHS);
//			int scanIndex = 0;
//			for(; scanIndex < klines.Count; scanIndex++){
//				if(klines[scanIndex].TxDate >= sinceDate) break;
//			}
//			if(scanIndex>=klines.Count) return result; //停牌时间超过SCAN_RANGE_IN_MONTHS
//			if(klines.Count - scanIndex < IGNORE_THRESHOLD) return result; //停牌时间过长导致剩余交易日较少
//			
//			SortedList topList = new SortedList(TOP_N_MIN);
//			for(int i=klines.Count - 1; i >= scanIndex; i--){
//				//排除一字板涨跌停的，因为通常是无量涨跌停
//				if(klines[i].IsAllDayOnLimitedPrice()) continue;
//				if(topList.ContainsKey(klines[i].Volume)) continue;
//				if(topList.Count<TOP_N_MIN){
//					topList.Add(klines[i].Volume, klines[i]);
//					continue;
//				}
//				if(klines[i].Volume < (long)topList.GetKey(TOP_N_MIN-1)){
//					topList.RemoveAt(TOP_N_MIN-1);
//					topList.Add(klines[i].Volume, klines[i]);
//				}
//			}
//			
//			HashSet<DateTime> matchDates = new HashSet<DateTime>();
//			for(int i=0; i<MATCH_RANGE_IN_DAYS; i++)
//				matchDates.Add(klines[klines.Count - i - 1].TxDate);
//			
//			for(int i=0; i<TOP_N_MIN; i++){
//				KJapaneseData kline = (KJapaneseData)topList.GetByIndex(i);
//				//换手率超过5%忽略
//				if(stock.CirculatingCapital>0 && kline.Volume * 1m / stock.CirculatingCapital >= 0.05m) break;
//				if(matchDates.Contains(kline.TxDate)){
//					result.Matched = true;
//					result.Type = this.TypeName;
//					result.Remark = kline.TxDate.ToString("yyyy-MM-dd") + " " 
//						+ (kline.Volume * 100m / stock.CirculatingCapital).ToString("F2") + "%";
//					return result;
//				}
//			}
//			
//			return result;
//		}
//	}
//}