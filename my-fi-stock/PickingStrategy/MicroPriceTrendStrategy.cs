using System;
using System.Collections.Generic;

using Pandora.Invest.Entity;
using Pandora.Invest.PickingStrategy;
using Pandora.Basis.DB;

namespace Pandora.Invest.PickingStrategy
{
	public class MicroPriceTrendStrategy : AbstractPickingStrategy
	{
		//震幅：amplitude，缩写为AM
		//AM-Open-Close：开盘价、收盘价涨跌幅【x%】以内
		//AM-Min-Max：日内振幅小于【x%】
		//AM-Prev：比上一交易日涨跌幅【x%】以内
		//Min-Matched-Days：需要连续匹配多少个十字星才符合该策略
		//Starting-Point：起始点必须在离当前日期多少个交易日以内
		
		public MicroPriceTrendStrategy() {}
		
		private class MatchingState{
			public MatchingState() {}
			public MatchingState(MatchingState target) {
				this.MatchedCount = target.MatchedCount;
				this.StartIndex = target.StartIndex;
				this.EndIndex = target.EndIndex;
				this.MaxClose = target.MaxClose;
				this.MinClose = target.MinClose;
				this.VolReduction = target.VolReduction;
				this.VolRatio = target.VolRatio;
			}
			
			public int MatchedCount = 0;
			public int StartIndex = -1;
			public int EndIndex = -1;
			
			public decimal MaxClose = 0;
			public decimal MinClose = 0;
			
			public bool VolReduction = false;
			public decimal VolRatio = 0;
			public DateTime VolPrevTopDate = DateTime.MinValue;
			
			public bool MAShortInc = false;
			public decimal MAShortRatio = 0;
			public DateTime MAShortTopDate = DateTime.MinValue;
			
			public bool MALongInc = false;
			public decimal MALongRatio = 0;
			public DateTime MALongTopDate = DateTime.MinValue;
			
			public IList<MatchingState> _matchedList = new List<MatchingState>();
			public IList<MatchingState> MatchedList { get{ return this._matchedList; } }
			
			public void MatchOne(int index, KJapaneseData kdata){
				this.MatchedCount++;
				if(this.EndIndex<0) this.EndIndex = index;
				this.StartIndex = index;
				if(this.MinClose<=0) this.MinClose = this.MaxClose = kdata.PriceClose;
				else{
					if(this.MinClose>kdata.PriceClose) this.MinClose = kdata.PriceClose;
					if(this.MaxClose<kdata.PriceClose) this.MaxClose = kdata.PriceClose;
				}
			}
			
			public void PushMatched(){
				this._matchedList.Add(new MatchingState(this));
			}
			
			public void Reset(){
				MatchedCount = 0;
				StartIndex = -1;
				EndIndex = -1;
				MaxClose = 0;
				MinClose = 0;
				VolReduction = false;
				VolRatio = 0;
			}
		}
		
		public override IList<PickingResult> DoPicking(StrategyConfig conf, Stock sto, IList<KJapaneseData> kdatas
		    	, IList<KTrendMAShort> masList, IList<KTrendMALong> malList, IList<KTrendVMALong> vmalList){
			decimal amOpenClose = conf.GetDecimal("AM-Open-Close");
			decimal amMinMax = conf.GetDecimal("AM-Min-Max");
			decimal amPrev = conf.GetDecimal("AM-Prev");
			decimal amMatchedDays = conf.GetDecimal("AM-Matched-Days");
			int minMatchedDays = conf.GetInt("Min-Matched-Days");
			int startingPoint = conf.GetInt("Starting-Point");
			
			decimal value1 = 0, value2 = 0 ;
			//微观价格条件匹配
			MatchingState state = new MatchingState();
			for(int i=kdatas.Count-1; i>0; i--){
				//价格为0无法计算，退出
				if(kdatas[i].PriceOpen==0 || kdatas[i].PriceMin==0 || kdatas[i].PriceClose==0) {
					log.Info(sto.StockCode + ": on " + kdatas[i].TxDate.ToString("yyyyMMdd") 
					         + "(open=" + kdatas[i].PriceOpen.ToString("f2") + ", max=" + kdatas[i].PriceMax.ToString("f2")
					         + ", min=" + kdatas[i].PriceMin.ToString("f2") + ", close=" + kdatas[i].PriceClose.ToString("f2")
					         + ", prev=" + kdatas[i].PricePrev.ToString("f2") + "), can't apply CrossStartLineStrategy");
					break; 
				}
				if(amOpenClose>0){ //开盘、收盘价振幅条件
					value1 = (kdatas[i].PriceClose - kdatas[i].PriceOpen) / kdatas[i].PriceOpen;
					if(Math.Abs(value1) > amOpenClose) { 
						StopMatching(state, minMatchedDays); 
						if( (kdatas.Count-1) - i + 1 >= startingPoint) break;
						continue;
					}
				}
				if(amMinMax>0){ //最高价、最低价振幅条件
					value1 = (kdatas[i].PriceMax - kdatas[i].PriceMin) / kdatas[i].PriceMin;
					if(value1 > amMinMax) { 
						StopMatching(state, minMatchedDays); 
						if( (kdatas.Count-1) - i + 1 >= startingPoint) break;
						continue;
					}
				}
				if(state.MatchedCount<=0){ 
					//还未匹配到任何节点，而单日条件已经判断完毕，则将该节点视作第一个满足条件的节点
					state.MatchOne(i, kdatas[i]);
					continue;
				}
				if(amPrev>0){ //相邻交易日收盘价振幅条件 
					//因为执行到此，state.MatchedCount>0必然成立，因此可以确保kdatas[i+1]不会越界
					value1 = (kdatas[i+1].PriceClose - kdatas[i].PriceClose) / kdatas[i].PriceClose;
					if(Math.Abs(value1) > amPrev){
						//这里不符合条件，是指已匹配节点与当前节点相邻日期间振幅不满足条件，因此对已匹配节点进行截止操作
						//而当前节点仍然是一个新的、有效的终止点
						StopMatching(state, minMatchedDays); 
						if( (kdatas.Count-1) - i + 1 >= startingPoint) break;
						state.MatchOne(i, kdatas[i]);
						continue;
					}
				}
				if(amMatchedDays>0) { //所有已匹配节点收盘价振幅条件
					if(kdatas[i].PriceClose<state.MinClose || kdatas[i].PriceClose>state.MaxClose){
						value1 = kdatas[i].PriceClose < state.MinClose ? kdatas[i].PriceClose : state.MinClose;
						value2 = kdatas[i].PriceClose > state.MaxClose ? kdatas[i].PriceClose : state.MaxClose;
						if( (value2 - value1) / value1 > amMatchedDays){
							StopMatching(state, minMatchedDays); 
							if( (kdatas.Count-1) - i + 1 >= startingPoint) break;
							state.MatchOne(i, kdatas[i]); //备注说明同amPrev
							continue;
						}
					}
				}
				state.MatchOne(i, kdatas[i]);
			}
			StopMatching(state, minMatchedDays);
			
			if(state.MatchedList.Count>0){
				//成交量趋势匹配
				foreach(MatchingState s in state.MatchedList){
					KTrendVMALong vma = base.FindMAData<KTrendVMALong>(vmalList, kdatas[s.StartIndex].TxDate, false);
					if(vma==null) continue;
					s.VolReduction = vma.EndValue < vma.StartValue;
					decimal vol = 0;
					for(int i=s.StartIndex; i<=s.EndIndex; i++){
						vol += kdatas[i].Volume;
					}
					KJapaneseData volTopData = base.FindKData(kdatas, vma.StartDate);
					s.VolRatio = (vol / s.MatchedCount) / volTopData.VMACusShort;
					s.VolPrevTopDate = volTopData.TxDate;
				}
				
				//后续价格走势匹配
				foreach(MatchingState s in state.MatchedList){
					KTrendMAShort mas = base.FindMAData<KTrendMAShort>(masList, kdatas[s.EndIndex].TxDate, true);
					if(mas!=null){
						s.MAShortInc = mas.EndValue > mas.StartValue;
						s.MAShortTopDate = mas.EndDate;
						s.MAShortRatio = (base.FindKData(kdatas, mas.EndDate).MACusShort - kdatas[s.EndIndex].PriceClose) / kdatas[s.EndIndex].PriceClose;
					}
					KTrendMALong mal = base.FindMAData<KTrendMALong>(malList, kdatas[s.EndIndex].TxDate, true);
					if(mal!=null){
						s.MALongInc = mal.EndValue > mal.StartValue;
						s.MALongTopDate = mal.EndDate;
						s.MALongRatio = (base.FindKData(kdatas, mal.EndDate).MACusShort - kdatas[s.EndIndex].PriceClose) / kdatas[s.EndIndex].PriceClose;
					}
				}
			}
			
			IList<PickingResult> result = new List<PickingResult>();
			foreach(MatchingState s in state.MatchedList){
				result.Add(new PickingResult(){
				    StockId = sto.StockId,
				    StartDate = kdatas[s.StartIndex].TxDate,
				    EndDate = kdatas[s.EndIndex].TxDate,
				    TxDays = s.MatchedCount,
				    VolDecrease = s.VolReduction,
				    VolRatio = s.VolRatio,
				    VolTopDate = s.VolPrevTopDate,
				    MAShortInc = s.MAShortInc,
				    MAShortIncRatio = s.MAShortRatio,
				    MAShortTopDate = s.MAShortTopDate,
				    MALongInc = s.MALongInc,
				    MALongIncRatio = s.MALongRatio,
				    MALongTopDate = s.MALongTopDate
				});
			}
			return result;
		}
		
		private void StopMatching(MatchingState state, int minDays){
			if(state.MatchedCount>=minDays) state.PushMatched();
			state.Reset();
		}
	}
}