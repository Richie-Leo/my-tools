using System;
using System.Collections.Generic;
using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using Pandora.Invest.MThread;

namespace Pandora.Invest.DataCapture
{
	public class CalKTrendWorker : MThreadWorker<Stock>{
		private Database _db;
		
		public override string LogPrefix {
			get {
				return "cal-ktrend";
			}
		}
		
		public override void BeforeDo(MThreadContext context)
		{
			string connectionString = context.Get("connection-string").ToString();
			this._db = new Database(connectionString);
			this._db.Open();
		}

        private void CalcIncSpeed(KTrend k){
            if (k.TxDays <= 1 || k.StartValue==0)
				return;
            k.ChangeSpeed = Convert.ToDecimal(Math.Pow(Convert.ToDouble(k.EndValue / k.StartValue), Convert.ToDouble(1.0 / (k.TxDays - 1))) - 1) * 100;
		}
		
		public override void Do(MThreadContext context, Stock item)
		{
			//TODO
			//if(item.StockId!=998) return;

			DateTime start = DateTime.Now;
			IList<KJapaneseData> kdatas = KJapaneseData.FindAll(this._db, item.StockId);
			if(kdatas.Count<=2) return;
			DateTime loaded=DateTime.Now;

            //to improve the KJapaneseData searching performance
            IDictionary<DateTime, KDataWrapper> wrappers = new Dictionary<DateTime, KDataWrapper>();
            for (int i=0; i<kdatas.Count; i++)
                wrappers.Add(kdatas[i].TxDate, new KDataWrapper() { Index=i, KData=kdatas[i] });
			
			List<KTrendMALong> plList = new List<KTrendMALong>();
			List<KTrendMAShort> psList = new List<KTrendMAShort>();
			List<KTrendVMALong> vlList = new List<KTrendVMALong>();
			bool plFlag = kdatas[1].MALong>kdatas[0].MALong
				, psFlag = kdatas[1].MAShort>kdatas[0].MAShort
				, vlFlag = kdatas[1].VMALong>kdatas[0].VMALong
				, upTrend;
			int plStart=0, psStart=0, vlStart=0, days;
			long min, max;
			KTrendVMALong vmaLong;
			KTrendMALong maLong;
			KTrendMAShort maShort;
			for(int i=1; i<kdatas.Count; i++){
				//短期价格趋势
				if(kdatas[i].MAShort!=kdatas[i-1].MAShort){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MAShort>kdatas[i-1].MAShort)!=psFlag){ //是否趋势转换节点
						maShort = new KTrendMAShort () {
							StockId = item.StockId, 
                            StartDate = kdatas [psStart].TxDate, EndDate = kdatas [i - 1].TxDate,
							StartValue = kdatas [psStart].ClosePrice, EndValue = kdatas [i - 1].ClosePrice,
							TxDays = (i - 1) - psStart + 1,
							ChangeSpeed = 0
						};
                        this.CalcIncSpeed(maShort);
						psList.Add(maShort);
						psFlag = !psFlag;
						psStart=i-1;
					}
				}
				//长期价格趋势
				if(kdatas[i].MALong!=kdatas[i-1].MALong){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MALong>kdatas[i-1].MALong)!=plFlag){ //是否趋势转换节点
						//plStart：该趋势区间起始索引；i-1：该趋势区间截止索引
						maLong = new KTrendMALong () {
							StockId = item.StockId, 
                            StartDate = kdatas [plStart].TxDate, EndDate = kdatas [i - 1].TxDate,
                            StartValue = kdatas [plStart].ClosePrice, EndValue = kdatas [i - 1].ClosePrice,
							TxDays = (i - 1) - plStart + 1,
							ChangeSpeed = 0 
						};
                        this.CalcIncSpeed(maLong);
						plList.Add(maLong);
						plFlag = !plFlag;
						plStart=i-1;
					}
				}
				//长期成交量趋势
				if(kdatas[i].VMALong!=kdatas[i-1].VMALong){ //成交量相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].VMALong>kdatas[i-1].VMALong)!=vlFlag){ //是否趋势转换节点
						vmaLong = new KTrendVMALong(){
							StockId=item.StockId, StartDate=kdatas[vlStart].TxDate, EndDate=kdatas[i-1].TxDate,
							StartValue=0, EndValue=0, TxDays = (i-1) - vlStart + 1, ChangeSpeed=0
					    };
						upTrend = kdatas[vlStart].Volume < kdatas[i-1].Volume;
						max = upTrend ? kdatas[i-1].Volume : kdatas[vlStart].Volume;
						min = upTrend ? kdatas[vlStart].Volume : kdatas[i-1].Volume;
						days=(i-1) - vlStart + 1;
						for(int j=vlStart; j<=i-1; j++){
							if(kdatas[j].IsAllDayOnFusingPrice()){
								days--;
								continue;
							}
							if(kdatas[j].Volume>max) max = kdatas[j].Volume;
							if(kdatas[j].Volume<min) min = kdatas[j].Volume;
						}
						vmaLong.StartValue = upTrend ? min : max;
						vmaLong.EndValue = upTrend ? max : min;
						vmaLong.TxDays = days <=0 ? (i-1) - vlStart + 1 : days;
                        this.CalcIncSpeed(vmaLong);
						vlList.Add(vmaLong);
						vlFlag = !vlFlag;
						vlStart=i-1;
					}
				}
			}

			maShort = new KTrendMAShort () {
				StockId = item.StockId, StartDate = kdatas [psStart].TxDate, EndDate = kdatas [kdatas.Count - 1].TxDate,
				StartValue = kdatas [psStart].ClosePrice, EndValue = kdatas [kdatas.Count - 1].ClosePrice,
				TxDays = (kdatas.Count - 1) - psStart + 1,
				ChangeSpeed = 0
			};
            this.CalcIncSpeed(maShort);
			psList.Add(maShort);

			maLong = new KTrendMALong () {
				StockId = item.StockId, 
                StartDate = kdatas [plStart].TxDate, EndDate = kdatas [kdatas.Count - 1].TxDate,
                StartValue = kdatas [plStart].ClosePrice, EndValue = kdatas [kdatas.Count - 1].ClosePrice,
				TxDays = (kdatas.Count - 1) - plStart + 1,
				ChangeSpeed = 0
			};
            this.CalcIncSpeed(maLong);
			plList.Add(maLong);
			
			vmaLong = new KTrendVMALong(){
				StockId=item.StockId, StartDate=kdatas[vlStart].TxDate, EndDate=kdatas[kdatas.Count-1].TxDate,
				StartValue=0, EndValue=0, TxDays = (kdatas.Count-1) - vlStart + 1, ChangeSpeed=0
		    };
			upTrend = kdatas[vlStart].Volume < kdatas[kdatas.Count-1].Volume;
			max = upTrend ? kdatas[kdatas.Count-1].Volume : kdatas[vlStart].Volume;
			min = upTrend ? kdatas[vlStart].Volume : kdatas[kdatas.Count-1].Volume;
			days=(kdatas.Count-1) - vlStart + 1;
			for(int j=vlStart; j<=kdatas.Count-1; j++){
				if(kdatas[j].IsAllDayOnFusingPrice()){
					days--;
					continue;
				}
				if(kdatas[j].Volume>max) max = kdatas[j].Volume;
				if(kdatas[j].Volume<min) min = kdatas[j].Volume;
			}
			vmaLong.StartValue = upTrend ? min : max;
			vmaLong.EndValue = upTrend ? max : min;
			vmaLong.TxDays = days <=0 ? (kdatas.Count-1) - vlStart + 1 : days;
            this.CalcIncSpeed(vmaLong);
			vlList.Add(vmaLong);
			DateTime caculated = DateTime.Now;

            plList = this.FindShortTermStrongMotion(plList, kdatas, wrappers, psList);
            this.ReviseVertexPosition(plList, kdatas, wrappers);
			
			KTrendMAShort.BatchImport(this._db, psList);
			KTrendMALong.BatchImport(this._db, plList);
			KTrendVMALong.BatchImport(this._db, vlList);
			
			TimeSpan ts1 = loaded-start, ts2 = caculated-loaded, ts3 = DateTime.Now - caculated;
			Info(item.StockCode + ", load:" + ts1.TotalMilliseconds.ToString("F0") 
			    + ", caculate:" + ts2.TotalMilliseconds.ToString("F0")
			    + ", insert:" + ts3.TotalMilliseconds.ToString("F0"));
		}

        private List<KTrendMALong> MergeShortSpanSections(List<KTrendMALong> listLong
            , IList<KJapaneseData> kdatas, IDictionary<DateTime, KDataWrapper> wrappers){
            int ShortSpanDays = 5;
            for (int i = 0; i < listLong.Count; i++){
            }
            return null;
        }

        private void ReviseVertexPosition(List<KTrendMALong> ma , IList<KJapaneseData> k, IDictionary<DateTime, KDataWrapper> w){
            for (int i = 0; i < ma.Count - 1;){ //对区间截止点进行调整，最后一个区间无需调整
                bool findMax = true, leftFirst = true;
                if (ma[i].Id <= 0 && ma[i + 1].Id <= 0){
                    //当前区间、下一区间都属于长期趋势中的区间
                    findMax = this.FindKData(w, ma[i].StartDate).MALong < this.FindKData(w, ma[i].EndDate).MALong;
                    //优先查找涨速较快的一侧
                    leftFirst = Math.Abs(ma[i].ChangeSpeed) > Math.Abs(ma[i + 1].ChangeSpeed);
                    this.ReviseVertexPosition(ma[i], ma[i + 1], findMax, leftFirst, k, w);
                    i++;
                    continue;
                }
                if (ma[i].Id <= 0 && ma[i + 1].Id > 0){
                    //当前区间属于长期趋势中的区间，下一区间属于短期趋势中的区间
                    findMax = ma[i+1].StartValue > ma[i+1].EndValue;
                    leftFirst = false;
                    this.ReviseVertexPosition(ma[i], ma[i + 1], findMax, leftFirst, k, w);
                    i++;
                    continue;
                }
                if (ma[i].Id > 0){
                    //当前区间属于短期趋势中的区间
                    findMax = ma[i].StartValue < ma[i+1].StartValue;
                    leftFirst = true;
                    this.ReviseVertexPosition(ma[i], ma[i + 1], findMax, leftFirst, k, w);
                    if (i + 2 < ma.Count && ma[i+2].Id<=0){
                        findMax = this.FindKData(w, ma[i+2].StartDate).MALong < this.FindKData(w, ma[i+2].EndDate).MALong;
                        leftFirst = Math.Abs(ma[i+1].ChangeSpeed) > Math.Abs(ma[i + 2].ChangeSpeed);
                        this.ReviseVertexPosition(ma[i+1], ma[i+2], findMax, leftFirst, k, w);
                    }
                    i += 2;
                    continue;
                }
                Error("Unrecognized trend zone, ignored", null);
                i++;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="current">当前均线区间</param>
        /// <param name="next">下一均线区间</param>
        /// <param name="findMax">是否从搜索范围中查找最大值作为更优顶点位置</param>
        /// <param name="leftFirst">是否优先从左侧查找</param>
        /// <param name="k">KJapanese data list</param>
        /// <param name="w">KJapanese data Dictionary wrapper</param>
        private void ReviseVertexPosition(KTrendMALong current, KTrendMALong next, bool findMax, bool leftFirst
            , IList<KJapaneseData> k, IDictionary<DateTime, KDataWrapper> w){
            int LookAroundDaysL = 5; //长期趋势区间，从前、后n天中查找更佳的顶点位置
            int LookAroundDaysS = 2; //短期趋势区间，从前、后n天中查找更佳的顶点位置
            decimal IgnoreRate = 0.03m; //顶点优先朝涨速较快的一侧调整，如果涨速较慢的一侧差异超过x%，则朝较慢一侧调整

            int lookAroundDays = current.Id > 0 || next.Id > 0 ? LookAroundDaysS : LookAroundDaysL;
            int index = this.FindKWrapper(w, current.EndDate).Index; //区间截止点在K线数据中的索引位置
            int match = index; //比截止点更适合的顶点位置

            int dr = leftFirst ? -1 : 1;
            //优先一侧
            for (int j = 1; j <= lookAroundDays; j++){
                if (index + j * dr < 0 || index + j * dr >= k.Count
                    || k[index + j * dr].TxDate <= current.StartDate || k[index + j * dr].TxDate >= next.EndDate)
                    break; //越界
                if (findMax){
                    if (k[index + j * dr].ClosePrice > k[match].ClosePrice)
                        match = index + j * dr;
                } else{
                    if (k[index + j * dr].ClosePrice < k[match].ClosePrice)
                        match = index + j * dr;
                }
            }
            //另外一侧
            dr = dr * -1;
            for (int j = 1; j <= lookAroundDays; j++){
                if (index + j * dr < 0 || index + j * dr >= k.Count
                    || k[index + j * dr].TxDate <= current.StartDate || k[index + j * dr].TxDate >= next.EndDate)
                    break; //越界
                if (findMax){
                    if (k[index + j * dr].ClosePrice > k[match].ClosePrice * (1 + IgnoreRate))
                        match = index + j * dr;
                } else{
                    if (k[index + j * dr].ClosePrice < k[match].ClosePrice * (1 - IgnoreRate))
                        match = index + j * dr;
                }
            }
            //fix vertex position
            if (match != index){
                Info("[move-vertex] [" + k[index].TxDate.ToString("yyMMdd") + " " + k[index].ClosePrice.ToString("f2")
                    + "] -> [" + k[match].TxDate.ToString("yyMMdd") + " " + k[match].ClosePrice.ToString("f2") + "]");
                current.EndDate = k[match].TxDate;
                current.EndValue = k[match].ClosePrice;
                current.TxDays = this.FindTxDays(w, current.StartDate, current.EndDate);
                this.CalcIncSpeed(current);

                next.StartDate = k[match].TxDate;
                next.StartValue = k[match].ClosePrice;
                next.TxDays = this.FindTxDays(w, next.StartDate, next.EndDate);
                this.CalcIncSpeed(next);
            }
        }

        private List<KTrendMALong> FindShortTermStrongMotion(List<KTrendMALong> listLong
            , IList<KJapaneseData> kdatas, IDictionary<DateTime, KDataWrapper> wrappers, List<KTrendMAShort> listShort){
			List<KTrendMALong> result = new List<KTrendMALong> (listLong.Count);
            List<KTrendMAShort> strongMotions = new List<KTrendMAShort>();
            //Find out strong fluctuations in the short-term trend
            decimal StrongMotionRateThreshold = 0.25m;
            decimal StrongMotionSpeedThreshold = 1m;
            decimal ShortVsLongMotionRatio = 0.65m;
            decimal ShortVsLongSpeedRatio = 2m;
            for(int i=0; i<listShort.Count; i++){
                KTrendMAShort es = listShort[i];
                if (Math.Abs((es.EndValue - es.StartValue) / es.StartValue) > StrongMotionRateThreshold 
                    && es.ChangeSpeed >= StrongMotionSpeedThreshold){
//                    if(DebugEnabled)
//                        Debug("[Strong-Motion] [" + es.StartDate.ToString("yyMMdd") + "->" + es.EndDate.ToString("yyMMdd")+
//                            " " + es.TxDays + " days] [AM:" + ((es.EndValue - es.StartValue) / es.StartValue * 100).ToString("f2") + 
//                            "%] [speed:" + es.IncSpeed.ToString("f2") + "]");
                    bool isDuplicated = false;
                    KTrendMALong containing = null;
                    foreach(KTrendMALong el in listLong){
                        if (!(es.StartDate >= el.EndDate || es.EndDate <= el.StartDate)){
                            int days1 = this.FindTxDays(wrappers
                                , es.StartDate<el.StartDate ? es.StartDate : el.StartDate
                                , es.StartDate<el.StartDate ? el.StartDate : es.StartDate);
                            int days2 = this.FindTxDays(wrappers
                                , es.EndDate<el.EndDate ? es.EndDate : el.EndDate
                                , es.EndDate<el.EndDate ? el.EndDate : es.EndDate);
                            if (days1 <= 4 && days2 <= 4){
                                isDuplicated = true;
                                break;
                            }
                        }
                        if (es.StartDate >= el.StartDate && es.EndDate <= el.EndDate){
                            if (i >= 2
                                && listShort[i - 1].StartDate >= el.StartDate && listShort[i - 1].EndDate <= el.EndDate
                                && listShort[i - 2].StartDate >= el.StartDate && listShort[i - 2].EndDate <= el.EndDate){
                                containing = el;
//                                if (DebugEnabled){
//                                    Debug("   [container] [" + el.StartDate.ToString("yyMMdd") + "->" + el.EndDate.ToString("yyMMdd") +
//                                        " " +el.TxDays+ " days] [AM:" + ((el.EndValue - el.StartValue) / el.StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + el.IncSpeed.ToString("f2") + "]");
//                                    Debug("   [es-1] [" + listShort[i - 1].StartDate.ToString("yyMMdd") + "->" + listShort[i - 1].EndDate.ToString("yyMMdd") +
//                                        " "+listShort[i - 1].TxDays+" days] [AM:" + ((listShort[i - 1].EndValue - listShort[i - 1].StartValue) / listShort[i - 1].StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + listShort[i - 1].IncSpeed.ToString("f2") + "]");
//                                    Debug("   [es-2] [" + listShort[i - 2].StartDate.ToString("yyMMdd") + "->" + listShort[i - 2].EndDate.ToString("yyMMdd") +
//                                        " "+listShort[i - 2].TxDays+" days] [AM:" + ((listShort[i - 2].EndValue - listShort[i - 2].StartValue) / listShort[i - 2].StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + listShort[i - 2].IncSpeed.ToString("f2")+"]");
//                                }
                                break;
                            }
                            if (i < listShort.Count - 2
                                && listShort[i + 1].StartDate >= el.StartDate && listShort[i + 1].EndDate <= el.EndDate
                                && listShort[i + 2].StartDate >= el.StartDate && listShort[i + 2].EndDate <= el.EndDate){
                                containing = el;
//                                if (DebugEnabled){
//                                    Debug("   [container] [" + el.StartDate.ToString("yyMMdd") + "->" + el.EndDate.ToString("yyMMdd") +
//                                        " " +el.TxDays+" days] [AM:" + ((el.EndValue - el.StartValue) / el.StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + el.IncSpeed.ToString("f2") + "]");
//                                    Debug("   [es+1] [" + listShort[i + 1].StartDate.ToString("yyMMdd") + "->" + listShort[i + 1].EndDate.ToString("yyMMdd") +
//                                        " " +listShort[i + 1].TxDays+" days] [AM:" + ((listShort[i + 1].EndValue - listShort[i + 1].StartValue) / listShort[i + 1].StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + listShort[i + 1].IncSpeed.ToString("f2")+ "]");
//                                    Debug("   [es+2] [" + listShort[i + 2].StartDate.ToString("yyMMdd") + "->" + listShort[i + 2].EndDate.ToString("yyMMdd") +
//                                        " " +listShort[i + 2].TxDays+" days] [AM:" + ((listShort[i + 2].EndValue - listShort[i + 2].StartValue) / listShort[i + 2].StartValue * 100).ToString("f2") +
//                                        "%] [speed:" + listShort[i + 2].IncSpeed.ToString("f2") + "]");
//                                }
                                break;
                            }
                        }
                    }
                    if (isDuplicated)
                        continue;
                    if (containing == null){
                        strongMotions.Add(es);
//                        if (DebugEnabled)
//                            Debug("   [OK] No container found, add to Strong-Shake list");
                    }else {
                        decimal amContainer = (containing.EndValue - containing.StartValue) / containing.StartValue;
                        decimal amES = (es.EndValue - es.StartValue) / es.StartValue;
                        if (amES / amContainer > ShortVsLongMotionRatio && es.ChangeSpeed / containing.ChangeSpeed > ShortVsLongSpeedRatio){
                            strongMotions.Add(es);
//                            if (DebugEnabled)
//                                Debug("   [OK] Container found, [FR:" + (amES / amContainer * 100).ToString("f2") + "%, "
//                                    + "SR:" + (es.IncSpeed / containing.IncSpeed * 100).ToString("f2")+"%]");
                        } else{
//                            if (DebugEnabled)
//                                Debug("   [Ignored] Container found, [FR:" + (amES / amContainer * 100).ToString("f2") + "%, "
//                                    + "SR:" + (es.IncSpeed / containing.IncSpeed * 100).ToString("f2")+"%]");
                        }
                    }
                }
            }
            //Repartition listLong
            for (int i = 0; i < listLong.Count; ){
                if (strongMotions.Count <= 0){
                    result.Add(listLong[i]);
                    i++;
                    continue;
                }
                if (listLong[i].EndDate <= strongMotions[0].StartDate){
                    result.Add(listLong[i]);
                    i++;
                    continue;
                }
                KTrendMALong malong;
                if (listLong[i].StartDate < strongMotions[0].StartDate){
                    malong = new KTrendMALong()
                    {
                        Id = 0,
                        StockId = listLong[i].StockId,
                        StartDate = listLong[i].StartDate,
                        StartValue = listLong[i].StartValue,
                        EndDate = strongMotions[0].StartDate,
                        EndValue = this.FindKData(wrappers, strongMotions[0].StartDate).ClosePrice,
                        TxDays = this.FindTxDays(wrappers, listLong[i].StartDate, strongMotions[0].StartDate)
                    };
                    this.CalcIncSpeed(malong);
                    result.Add(malong);
                }
                if (listLong[i].EndDate < strongMotions[0].EndDate){
                    i++;
                    continue;
                }
                malong = new KTrendMALong()
                {
                    Id = 99,
                    StockId = strongMotions[0].StockId,
                    StartDate = strongMotions[0].StartDate,
                    StartValue = this.FindKData(wrappers, strongMotions[0].StartDate).ClosePrice,
                    EndDate = strongMotions[0].EndDate,
                    EndValue = this.FindKData(wrappers, strongMotions[0].EndDate).ClosePrice,
                    TxDays = this.FindTxDays(wrappers, strongMotions[0].StartDate, strongMotions[0].EndDate)
                };
                this.CalcIncSpeed(malong);
                Info("[strong-motion] [" + malong.StartDate.ToString("yyMMdd") 
                    + " > " + malong.EndDate.ToString("yyMMdd") + " " + malong.TxDays + " days] [AM:" 
                    + ((malong.EndValue - malong.StartValue) / malong.StartValue * 100).ToString("f2") + "] [speed:" 
                    + malong.ChangeSpeed.ToString("f2") + "]");
                result.Add(malong);
                if (listLong[i].StartDate < malong.EndDate){
                    listLong[i].StartDate = malong.EndDate;
                    listLong[i].StartValue = malong.EndValue;
                    listLong[i].TxDays = this.FindTxDays(wrappers, listLong[i].StartDate, listLong[i].EndDate);
                    this.CalcIncSpeed(listLong[i]);
                }
                strongMotions.RemoveAt(0);
            }
            //Merging listLong: merge neighbours both amplitude is under 10%
            //震荡上行/下行，合并成一个长期趋势
//            for (int i = 0; i < result.Count - 1; ){
//                //判断是否应该将i、i+1合并
//                if (result[i].Id > 0 || result[i + 1].Id > 0){ //不针对切分区域进行合并处理
//                    i++;
//                    continue;
//                }
//            }
            //合并相邻距离短、涨跌幅小的区间

			return result;
		}

        private KJapaneseData FindKData(IDictionary<DateTime, KDataWrapper> wrappers, DateTime date){
            if (wrappers.ContainsKey(date))
                return wrappers[date].KData;
            return null;
        }

        private KDataWrapper FindKWrapper(IDictionary<DateTime, KDataWrapper> wrappers, DateTime date){
            if (wrappers.ContainsKey(date))
                return wrappers[date];
            return null;
        }

        private int FindTxDays(IDictionary<DateTime, KDataWrapper> wrappers, DateTime start, DateTime end){
            KDataWrapper startWrapper = this.FindKWrapper(wrappers, start);
            KDataWrapper endWrapper = this.FindKWrapper(wrappers, end);
            if (startWrapper == null || endWrapper == null)
                return 1;
            return endWrapper.Index - startWrapper.Index + 1;
        }

        private class KDataWrapper{
            public KJapaneseData KData { get; set; }
            public int Index { get; set; }
        }
		
		public override void AfterDo(MThreadContext context)
		{
			this._db.Close();
		}
	}
}