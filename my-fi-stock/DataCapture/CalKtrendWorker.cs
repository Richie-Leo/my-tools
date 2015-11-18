﻿using System;
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
		
		public override void Do(MThreadContext context, Stock item)
		{
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
			for(int i=1; i<kdatas.Count; i++){
				//长期价格趋势
				if(kdatas[i].MACusLong!=kdatas[i-1].MACusLong){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MACusLong>kdatas[i-1].MACusLong)!=plFlag){ //是否趋势转换节点
						//plStart：该趋势区间起始索引；i-1：该趋势区间截止索引
						plList.Add(new KTrendMALong(){
							StockId=item.StockId, StartDate=kdatas[plStart].TxDate, EndDate=kdatas[i-1].TxDate,
							StartValue=kdatas[plStart].PriceClose, EndValue=kdatas[i-1].PriceClose,
							TxDays = (i-1) - plStart + 1,
							IncSpeed=(kdatas[i-1].PriceClose - kdatas[plStart].PriceClose) / kdatas[plStart].PriceClose / ((i-1) - plStart + 1) * 100
					    });
						plFlag = !plFlag;
						plStart=i-1;
					}
				}
				//短期价格趋势
				if(kdatas[i].MACusShort!=kdatas[i-1].MACusShort){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
					if((kdatas[i].MACusShort>kdatas[i-1].MACusShort)!=psFlag){ //是否趋势转换节点
						psList.Add(new KTrendMAShort(){
							StockId=item.StockId, StartDate=kdatas[psStart].TxDate, EndDate=kdatas[i-1].TxDate,
							StartValue=kdatas[psStart].PriceClose, EndValue=kdatas[i-1].PriceClose,
							TxDays = (i-1) - psStart + 1,
							IncSpeed=(kdatas[i-1].PriceClose - kdatas[psStart].PriceClose) / kdatas[psStart].PriceClose / ((i-1) - psStart + 1) * 100
					    });
						psFlag = !psFlag;
						psStart=i-1;
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
						vmaLong.IncSpeed = (vmaLong.EndValue-vmaLong.StartValue) / vmaLong.StartValue / vmaLong.TxDays * 100;
						vlList.Add(vmaLong);
						vlFlag = !vlFlag;
						vlStart=i-1;
					}
				}
			}
			
			plList.Add(new KTrendMALong(){
				StockId=item.StockId, StartDate=kdatas[plStart].TxDate, EndDate=kdatas[kdatas.Count-1].TxDate,
				StartValue=kdatas[plStart].PriceClose, EndValue=kdatas[kdatas.Count-1].PriceClose,
				TxDays = (kdatas.Count-1) - plStart + 1,
				IncSpeed=(kdatas[kdatas.Count-1].PriceClose - kdatas[plStart].PriceClose) / kdatas[plStart].PriceClose / ((kdatas.Count-1) - plStart + 1) * 100
		    });
			
			psList.Add(new KTrendMAShort(){
				StockId=item.StockId, StartDate=kdatas[psStart].TxDate, EndDate=kdatas[kdatas.Count-1].TxDate,
				StartValue=kdatas[psStart].PriceClose, EndValue=kdatas[kdatas.Count-1].PriceClose,
				TxDays = (kdatas.Count-1) - psStart + 1,
				IncSpeed=(kdatas[kdatas.Count-1].PriceClose - kdatas[psStart].PriceClose) / kdatas[plStart].PriceClose / ((kdatas.Count-1) - psStart + 1) * 100
		    });
			
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
			vmaLong.IncSpeed = (vmaLong.EndValue-vmaLong.StartValue) / vmaLong.StartValue / vmaLong.TxDays * 100;
			vlList.Add(vmaLong);
			DateTime caculated = DateTime.Now;
			
			KTrendMAShort.BatchImport(this._db, psList);
			KTrendMALong.BatchImport(this._db, plList);
			KTrendVMALong.BatchImport(this._db, vlList);
			
			TimeSpan ts1 = loaded-start, ts2 = caculated-loaded, ts3 = DateTime.Now - caculated;
			Info(item.StockCode + ", load:" + ts1.TotalMilliseconds.ToString("F0") 
			    + ", caculate:" + ts2.TotalMilliseconds.ToString("F0")
			    + ", insert:" + ts3.TotalMilliseconds.ToString("F0"));
		}
		
		public override void AfterDo(MThreadContext context)
		{
			this._db.Close();
		}
	}
}