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

		private decimal CalcIncSpeed(decimal s, decimal e, int n){
			if (n <= 1 || s==0)
				return 0;
			return Convert.ToDecimal(Math.Pow(Convert.ToDouble(e/s), Convert.ToDouble(1.0/(n-1))) - 1) * 100;
		}
		
		public override void Do(MThreadContext context, Stock item)
		{
			//TODO
			if(item.StockId!=998 && item.StockId!=600378) return;

			DateTime start = DateTime.Now;
			IList<KJapaneseData> kdatas = KJapaneseData.FindAll(this._db, item.StockId);
			if(kdatas.Count<=2) return;
			DateTime loaded=DateTime.Now;
			
			List<KTrendMALong> plList = new List<KTrendMALong>();
			List<KTrendMAShort> psList = new List<KTrendMAShort>();
			List<KTrendVMALong> vlList = new List<KTrendVMALong>();
			bool plFlag = kdatas[1].MACusLong>kdatas[0].MACusLong
				, psFlag = kdatas[1].MACusShort>kdatas[0].MACusShort
				, vlFlag = kdatas[1].VMACusLong>kdatas[0].VMACusLong
				, upTrend;
			int plStart=0, psStart=0, vlStart=0, days;
			long min, max;
			KTrendVMALong vmaLong;
			KTrendMALong maLong;
			KTrendMAShort maShort;
			for(int i=1; i<kdatas.Count; i++){
				//短期价格趋势
				if(kdatas[i].MACusShort!=kdatas[i-1].MACusShort){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MACusShort>kdatas[i-1].MACusShort)!=psFlag){ //是否趋势转换节点
						maShort = new KTrendMAShort () {
							StockId = item.StockId, StartDate = kdatas [psStart].TxDate, EndDate = kdatas [i - 1].TxDate,
							StartValue = kdatas [psStart].PriceClose, EndValue = kdatas [i - 1].PriceClose,
							TxDays = (i - 1) - psStart + 1,
							IncSpeed = 0
						};
						maShort.IncSpeed = this.CalcIncSpeed(maShort.StartValue, maShort.EndValue, maShort.TxDays);
						psList.Add(maShort);
						psFlag = !psFlag;
						psStart=i-1;
					}
				}
				//长期价格趋势
				if(kdatas[i].MACusLong!=kdatas[i-1].MACusLong){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MACusLong>kdatas[i-1].MACusLong)!=plFlag){ //是否趋势转换节点
						//plStart：该趋势区间起始索引；i-1：该趋势区间截止索引
						maLong = new KTrendMALong () {
							StockId = item.StockId, StartDate = kdatas [plStart].TxDate, EndDate = kdatas [i - 1].TxDate,
							StartValue = kdatas [plStart].MACusShort, EndValue = kdatas [i - 1].MACusShort,
							TxDays = (i - 1) - plStart + 1,
							IncSpeed = 0 
						};
						maLong.IncSpeed = this.CalcIncSpeed (maLong.StartValue, maLong.EndValue, maLong.TxDays);
						plList.Add(maLong);
						plFlag = !plFlag;
						plStart=i-1;
					}
				}
				//长期成交量趋势
				if(kdatas[i].VMACusLong!=kdatas[i-1].VMACusLong){ //成交量相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].VMACusLong>kdatas[i-1].VMACusLong)!=vlFlag){ //是否趋势转换节点
						vmaLong = new KTrendVMALong(){
							StockId=item.StockId, StartDate=kdatas[vlStart].TxDate, EndDate=kdatas[i-1].TxDate,
							StartValue=0, EndValue=0, TxDays = (i-1) - vlStart + 1, IncSpeed=0
					    };
						upTrend = kdatas[vlStart].Volume < kdatas[i-1].Volume;
						max = upTrend ? kdatas[i-1].Volume : kdatas[vlStart].Volume;
						min = upTrend ? kdatas[vlStart].Volume : kdatas[i-1].Volume;
						days=(i-1) - vlStart + 1;
						for(int j=vlStart; j<=i-1; j++){
							if(kdatas[j].IsAllDayOnLimitedPrice()){
								days--;
								continue;
							}
							if(kdatas[j].Volume>max) max = kdatas[j].Volume;
							if(kdatas[j].Volume<min) min = kdatas[j].Volume;
						}
						vmaLong.StartValue = upTrend ? min : max;
						vmaLong.EndValue = upTrend ? max : min;
						vmaLong.TxDays = days <=0 ? (i-1) - vlStart + 1 : days;
						vmaLong.IncSpeed = this.CalcIncSpeed(vmaLong.StartValue, vmaLong.EndValue, vmaLong.TxDays);
						vlList.Add(vmaLong);
						vlFlag = !vlFlag;
						vlStart=i-1;
					}
				}
			}

			maShort = new KTrendMAShort () {
				StockId = item.StockId, StartDate = kdatas [psStart].TxDate, EndDate = kdatas [kdatas.Count - 1].TxDate,
				StartValue = kdatas [psStart].PriceClose, EndValue = kdatas [kdatas.Count - 1].PriceClose,
				TxDays = (kdatas.Count - 1) - psStart + 1,
				IncSpeed = 0
			};
			maShort.IncSpeed = this.CalcIncSpeed (maShort.StartValue, maShort.EndValue, maShort.TxDays);
			psList.Add(maShort);

			maLong = new KTrendMALong () {
				StockId = item.StockId, StartDate = kdatas [plStart].TxDate, EndDate = kdatas [kdatas.Count - 1].TxDate,
				StartValue = kdatas [plStart].MACusShort, EndValue = kdatas [kdatas.Count - 1].MACusShort,
				TxDays = (kdatas.Count - 1) - plStart + 1,
				IncSpeed = 0
			};
			maLong.IncSpeed = this.CalcIncSpeed (maLong.StartValue, maLong.EndValue, maLong.TxDays);
			plList.Add(maLong);
			
			vmaLong = new KTrendVMALong(){
				StockId=item.StockId, StartDate=kdatas[vlStart].TxDate, EndDate=kdatas[kdatas.Count-1].TxDate,
				StartValue=0, EndValue=0, TxDays = (kdatas.Count-1) - vlStart + 1, IncSpeed=0
		    };
			upTrend = kdatas[vlStart].Volume < kdatas[kdatas.Count-1].Volume;
			max = upTrend ? kdatas[kdatas.Count-1].Volume : kdatas[vlStart].Volume;
			min = upTrend ? kdatas[vlStart].Volume : kdatas[kdatas.Count-1].Volume;
			days=(kdatas.Count-1) - vlStart + 1;
			for(int j=vlStart; j<=kdatas.Count-1; j++){
				if(kdatas[j].IsAllDayOnLimitedPrice()){
					days--;
					continue;
				}
				if(kdatas[j].Volume>max) max = kdatas[j].Volume;
				if(kdatas[j].Volume<min) min = kdatas[j].Volume;
			}
			vmaLong.StartValue = upTrend ? min : max;
			vmaLong.EndValue = upTrend ? max : min;
			vmaLong.TxDays = days <=0 ? (kdatas.Count-1) - vlStart + 1 : days;
			vmaLong.IncSpeed = this.CalcIncSpeed (vmaLong.StartValue, vmaLong.EndValue, vmaLong.TxDays);
			vlList.Add(vmaLong);
			DateTime caculated = DateTime.Now;

            plList = this.FixMALong (kdatas, plList, psList);
			
			KTrendMAShort.BatchImport(this._db, psList);
			KTrendMALong.BatchImport(this._db, plList);
			KTrendVMALong.BatchImport(this._db, vlList);
			
			TimeSpan ts1 = loaded-start, ts2 = caculated-loaded, ts3 = DateTime.Now - caculated;
			Info(item.StockCode + ", load:" + ts1.TotalMilliseconds.ToString("F0") 
			    + ", caculate:" + ts2.TotalMilliseconds.ToString("F0")
			    + ", insert:" + ts3.TotalMilliseconds.ToString("F0"));
		}

        private List<KTrendMALong> FixMALong(IList<KJapaneseData> kdatas, List<KTrendMALong> listLong, List<KTrendMAShort> listShort){
			List<KTrendMALong> result = new List<KTrendMALong> (listLong.Count);
            List<KTrendMAShort> strongShakes = new List<KTrendMAShort>();
            //Find out strong shakes in short period
            for(int i=0; i<listShort.Count; i++){
                KTrendMAShort es = listShort[i];
                if (Math.Abs((es.EndValue - es.StartValue) / es.StartValue) > 0.2m && es.IncSpeed >= 1){
                    KTrendMALong containing = null;
                    foreach(KTrendMALong el in listLong){
                        if (es.StartDate >= el.StartDate && es.EndDate <= el.EndDate){
                            if (i >= 2
                                && listShort[i - 1].StartDate >= el.StartDate && listShort[i - 1].EndDate <= el.EndDate
                                && listShort[i - 2].StartDate >= el.StartDate && listShort[i - 2].EndDate <= el.EndDate){
                                containing = el;
                                break;
                            }
                            if (i < listShort.Count - 2
                                && listShort[i + 1].StartDate >= el.StartDate && listShort[i + 1].EndDate <= el.EndDate
                                && listShort[i + 2].StartDate >= el.StartDate && listShort[i + 2].EndDate <= el.EndDate){
                                containing = el;
                                break;
                            }
                        }
                    }
                    if (containing != null)
                        strongShakes.Add(es);
                }
            }
            //Repartition listLong
            for (int i = 0; i < listLong.Count; ){
                if (strongShakes.Count <= 0){
                    result.Add(listLong[i]);
                    i++;
                    continue;
                }
                if (listLong[i].EndDate <= strongShakes[0].StartDate){
                    result.Add(listLong[i]);
                    i++;
                    continue;
                }
                KTrendMALong malong;
                if (listLong[i].StartDate < strongShakes[0].StartDate){
                    malong = new KTrendMALong()
                    {
                        Id = 0,
                        StockId = listLong[i].StockId,
                        StartDate = listLong[i].StartDate,
                        StartValue = listLong[i].StartValue,
                        EndDate = strongShakes[0].StartDate,
                        EndValue = this.FindKData(kdatas, strongShakes[0].StartDate).MACusShort,
                        TxDays = this.FindTxDays(kdatas, listLong[i].StartDate, strongShakes[0].StartDate)
                    };
                    malong.IncSpeed = this.CalcIncSpeed(malong.StartValue, malong.EndValue, malong.TxDays);
                    result.Add(malong);
                }
                if (listLong[i].EndDate < strongShakes[0].EndDate){
                    i++;
                    continue;
                }
                malong = new KTrendMALong()
                {
                    Id = 0,
                    StockId = strongShakes[0].StockId,
                    StartDate = strongShakes[0].StartDate,
                    StartValue = this.FindKData(kdatas, strongShakes[0].StartDate).MACusShort,
                    EndDate = strongShakes[0].EndDate,
                    EndValue = this.FindKData(kdatas, strongShakes[0].EndDate).MACusShort,
                    TxDays = this.FindTxDays(kdatas, strongShakes[0].StartDate, strongShakes[0].EndDate)
                };
                malong.IncSpeed = this.CalcIncSpeed(malong.StartValue, malong.EndValue, malong.TxDays);
                result.Add(malong);
                if (listLong[i].StartDate < malong.EndDate){
                    listLong[i].StartDate = malong.EndDate;
                    listLong[i].StartValue = malong.EndValue;
                    listLong[i].TxDays = this.FindTxDays(kdatas, listLong[i].StartDate, listLong[i].EndDate);
                    listLong[i].IncSpeed = this.CalcIncSpeed(listLong[i].StartValue, listLong[i].EndValue, listLong[i].TxDays);
                }
                strongShakes.RemoveAt(0);
            }
            //Merging listLong: merge neighbours both amplitude is under 10%
            for (int i = 0; i < result.Count-1; ){
                decimal am1 = (result[i].EndValue - result[i].StartValue) / result[i].StartValue;
                decimal am2 = (result[i+1].EndValue - result[i+1].StartValue) / result[i+1].StartValue;
                if (Math.Abs(am1) < 0.1m && Math.Abs(am2) < 0.1m){
                    KTrendMALong newLong = new KTrendMALong()
                    {
                            Id = 0,
                            StockId = result[i].StockId,
                            StartDate = result[i].StartDate,
                            StartValue = result[i].StartValue,
                            EndDate = result[i+1].EndDate,
                            EndValue = result[i+1].EndValue,
                            TxDays = result[i].TxDays + result[i+1].TxDays -1
                    };
                    newLong.IncSpeed = this.CalcIncSpeed(newLong.StartValue, newLong.EndValue, newLong.TxDays);
                    result[i] = newLong;
                    result.RemoveAt(i + 1);
                    continue;
                }
                i++;
            }

			return result;
		}

        private KJapaneseData FindKData(IList<KJapaneseData> kdatas, DateTime date){
            foreach (KJapaneseData k in kdatas)
                if (k.TxDate == date)
                    return k;
            return null;
        }

        private int FindTxDays(IList<KJapaneseData> kdatas, DateTime start, DateTime end){
            int result = 0;
            foreach (KJapaneseData k in kdatas){
                if (k.TxDate < start)
                    continue;
                if (k.TxDate > end)
                    break;
                result++;
            }
            return result;
        }
		
		public override void AfterDo(MThreadContext context)
		{
			this._db.Close();
		}
	}
}