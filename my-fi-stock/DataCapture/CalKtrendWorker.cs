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
		
		public override void Do(MThreadContext context, Stock item)
		{
			//TODO
			if(item.StockId!=600621) return;

            //TODO
            //1. 合并2个连续上涨、下跌，且涨速、跌速相差不大的区间
            //2. 一个区间中 hi > max(s, e) 或者 lo < min(s, e),（且差距较大？），重新调整s、e
            //3. 

			DateTime start = DateTime.Now;
            IList<KJapaneseData> lk = KJapaneseData.FindAll(this._db, item.StockId);
			if(lk.Count<=2) return;
			DateTime loaded=DateTime.Now;

            //to improve the KJapaneseData searching performance
            IDictionary<DateTime, KDataWrapper> dw = new Dictionary<DateTime, KDataWrapper>();
            for (int i=0; i<lk.Count; i++)
                dw.Add(lk[i].TxDate, new KDataWrapper() { Index=i, KData=lk[i] });
			
            List<KTrendMALong> lmal = new List<KTrendMALong>();
            List<KTrendMAShort> lmas = new List<KTrendMAShort>();
            List<KTrendVMALong> lvmal = new List<KTrendVMALong>();

            this.BuildKTrend(lmas, lmal, lvmal, item, lk, dw);
            lmal = this.Split(lmal, lk, dw, lmas);
            this.ReviseVertexPosition(lmal, lk, dw);
            this.Merge(lmal, dw);

            DateTime caculated = DateTime.Now;
			
			KTrendMAShort.BatchImport(this._db, lmas);
			KTrendMALong.BatchImport(this._db, lmal);
			KTrendVMALong.BatchImport(this._db, lvmal);
			
			TimeSpan ts1 = loaded-start, ts2 = caculated-loaded, ts3 = DateTime.Now - caculated;
			Info(item.StockCode + ", load:" + ts1.TotalMilliseconds.ToString("F0") 
			    + ", caculate:" + ts2.TotalMilliseconds.ToString("F0")
			    + ", insert:" + ts3.TotalMilliseconds.ToString("F0"));
		}

        public override void AfterDo(MThreadContext context)
        {
            this._db.Close();
        }

        #region BuildKTrend() 构建趋势区间
        /// <summary>
        /// 构建趋势区间，构建好的结果分别放在入参lmas、lmal、lvmal中
        /// </summary>
        /// <param name="lmas">输出：短期均价趋势区间</param>
        /// <param name="lmal">输出：长期均价趋势区间</param>
        /// <param name="lvmal">输出：长期均量趋势区间</param>
        /// <param name="stock"></param>
        /// <param name="lk">K线数据: 列表</param>
        /// <param name="dw">K线数据: 字典</param>
        private void BuildKTrend(List<KTrendMAShort> lmas, List<KTrendMALong> lmal, List<KTrendVMALong> lvmal
            , Stock stock, IList<KJapaneseData> lk, IDictionary<DateTime, KDataWrapper> dw){
            int FragmentIntervalDays = 8;
            decimal FragmentIntervalNC = 0.01m;

            bool malf = lk[1].MALong > lk[0].MALong, masf = lk[1].MAShort > lk[0].MAShort, vmalf = lk[1].VMALong > lk[0].VMALong;
            int mali = 0, masi = 0, vmali = 0;
            KTrendMALong mal;
            for (int i = 1; i < lk.Count; i++){
                //短期价格趋势
                if (lk[i].MAShort != lk[i - 1].MAShort){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
                    if ((lk[i].MAShort > lk[i - 1].MAShort) != masf){ //是否趋势转换节点
                        lmas.Add(this.BuildTrend<KTrendMAShort>(MAType.MAShort, lk, masi, i - 1));
                        masf = !masf;
                        masi = i - 1;
                    }
                }
                //长期价格趋势
                if (lk[i].MALong != lk[i - 1].MALong){ //价格相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
                    if ((lk[i].MALong > lk[i - 1].MALong) != malf){ //是否趋势转换节点
                        //plStart：该趋势区间起始索引；i-1：该趋势区间截止索引
                        mal = this.BuildTrend<KTrendMALong>(MAType.MALong, lk, mali, i - 1);
                        decimal malNC = KTrend.CalNetChange(lk[mali].MALong, lk[i - 1].MALong);
                        //忽略掉时间跨度短，且振幅不大的区间
                        if (mal.TxDays > FragmentIntervalDays || Math.Abs(malNC) > FragmentIntervalNC){
                            lmal.Add(mal);
                            mali = i - 1;
                        }
                        malf = !malf;
                    }
                }
                //长期成交量趋势
                if (lk[i].VMALong != lk[i - 1].VMALong){ //成交量相等，则包含在当前趋势区间中，不相等时才进行趋势转换判断
                    if ((lk[i].VMALong > lk[i - 1].VMALong) != vmalf){ //是否趋势转换节点
                        lvmal.Add(this.BuildTrend<KTrendVMALong>(MAType.VMALong, lk, vmali, i - 1));
                        vmalf = !vmalf;
                        vmali = i - 1;
                    }
                }
            }

            lmas.Add(this.BuildTrend<KTrendMAShort>(MAType.MAShort, lk, masi, lk.Count - 1));
            lmal.Add(this.BuildTrend<KTrendMALong>(MAType.MALong, lk, mali, lk.Count - 1));
            lvmal.Add(this.BuildTrend<KTrendVMALong>(MAType.VMALong, lk, vmali, lk.Count - 1));
        }

        public T BuildTrend<T>(MAType type, IList<KJapaneseData> lk, int start, int end) where T: KTrend
        {
            T result = default(T);
            KTrend trend = null;
            try{
                result = (T)Activator.CreateInstance(typeof(T));
            }catch(Exception ex){
                base.Error("无法创建" + typeof(T).Name, ex);
                return default(T);
            }
            trend = (KTrend)result;

            trend.StockId = lk[start].StockId;
            trend.StartDate = lk[start].TxDate;
            trend.EndDate = lk[end].TxDate;
            trend.TxDays = end - start + 1;

            switch (type){
                case MAType.MAShort:
                case MAType.MALong:
                    trend.StartValue = lk[start].ClosePrice;
                    trend.EndValue = lk[end].ClosePrice;
                    trend.Remark = "MAL";
                    break;
                case MAType.VMAShort:
                case MAType.VMALong:
                    trend.StartValue = lk[start].Volume;
                    trend.EndValue = lk[end].Volume;
                    trend.Remark = "VMAL";
                    break;
            }

            trend.NetChange = KTrend.CalNetChange(trend.StartValue, trend.EndValue);
            KTrend.CalHighLowValue(trend, type, lk, start, end);
            KTrend.CalChangeSpeed(trend);

            return trend as T;
        }
        #endregion

        #region Merge() 区间合并 (长期价格趋势)
        /// <summary>
        /// 区间合并 (长期价格趋势)，直接修改入参lmal
        /// </summary>
        /// <param name="lmal">输入、输出：K线数据: 列表</param>
        /// <param name="dw">K线数据: 字典</param>
        private void Merge(List<KTrendMALong> lmal, IDictionary<DateTime, KDataWrapper> dw){
            #region Fragment Merge: 合并时间跨度较小的区间
            int DaysForFragmentMerge = 5;
            decimal NCForFragmentMerge = 0.05m;
            for (int i = 0; i < lmal.Count; ){
                if (lmal[i].TxDays > DaysForFragmentMerge || Math.Abs(lmal[i].NetChange) > NCForFragmentMerge){
                    i++;
                    continue; //不满足合并条件，继续
                }
            	//满足合并条件，判断向前还是向后合并:
                int direction = 0; //-1:向前合并；1:向后合并
                if (i == 0) //第一个区间，只能向后合并
                    direction = 1;
                else if (i == lmal.Count - 1) //最后一个区间，只能向前合并
                    direction = -1;
                else{ //不是第一个、最后一个区间，则：
                	//1.禁止向拆分区间合并;
	                //2.前区间和后区间方向相反，则将待合并区间向同向区间合并;
	                //3.前区间和后区间方向相同，则将待合并区间向涨速较慢的区间合并;
                    if(lmal[i-1].Id>0) direction = 1;
                    else if(lmal[i+1].Id>0) direction = -1;
                    else{
                        if((lmal[i].ChangeSpeed>0 && lmal[i-1].ChangeSpeed>0) || (lmal[i].ChangeSpeed<0 && lmal[i-1].ChangeSpeed<0))
                            direction = -1;
                        else if((lmal[i].ChangeSpeed>0 && lmal[i+1].ChangeSpeed>0) || (lmal[i].ChangeSpeed<0 && lmal[i+1].ChangeSpeed<0))
                            direction = -1;
                        else{
                            direction = Math.Abs(lmal[i-1].ChangeSpeed) < Math.Abs(lmal[i+1].ChangeSpeed) ? -1 : 1;
                        }
                    }
                }
                if(direction>0 && i+1<lmal.Count && lmal[i+1].Id<=0){
                    lmal[i].EndDate = lmal[i+1].EndDate;
                    lmal[i].EndValue = lmal[i+1].EndValue;
                    lmal[i].TxDays = this.FindTxDays(dw, lmal[i].StartDate, lmal[i].EndDate);
                    lmal[i].HighValue = lmal[i].HighValue > lmal[i+1].HighValue ? lmal[i].HighValue : lmal[i+1].HighValue;
                    lmal[i].LowValue = lmal[i].LowValue < lmal[i+1].LowValue ? lmal[i].LowValue : lmal[i+1].LowValue;
                    lmal[i].Amplitude = KTrend.CalNetChange(lmal[i].LowValue, lmal[i].HighValue);
                    lmal[i].NetChange = KTrend.CalNetChange(lmal[i].StartValue, lmal[i].EndValue);
                    KTrend.CalChangeSpeed(lmal[i]);
                    lmal[i].Remark += "; Merge(" + lmal[i+1].StartDate.ToString("yyyyMMdd") + "-" + lmal[i+1].EndDate.ToString("yyyyMMdd") + ")";
                    lmal.RemoveAt(i+1);
                    //合并之后不改变索引值，下次循环继续判断合并后的区间是否仍满足合并条件
                    continue;
                }else if(direction<0 && i>=1 && lmal[i-1].Id<=0){
                    lmal[i-1].EndDate = lmal[i].EndDate;
                    lmal[i-1].EndValue = lmal[i].EndValue;
                    lmal[i-1].TxDays = this.FindTxDays(dw, lmal[i-1].StartDate, lmal[i-1].EndDate);
                    lmal[i-1].HighValue = lmal[i-1].HighValue > lmal[i].HighValue ? lmal[i-1].HighValue : lmal[i].HighValue;
                    lmal[i-1].LowValue = lmal[i-1].LowValue < lmal[i].LowValue ? lmal[i-1].LowValue : lmal[i].LowValue;
                    lmal[i-1].Amplitude = KTrend.CalNetChange(lmal[i-1].LowValue, lmal[i-1].HighValue);
                    lmal[i-1].NetChange = KTrend.CalNetChange(lmal[i-1].StartValue, lmal[i-1].EndValue);
                    KTrend.CalChangeSpeed(lmal[i-1]);
                    lmal[i-1].Remark += "; Merge(" + lmal[i].StartDate.ToString("yyyyMMdd") + "-" + lmal[i].EndDate.ToString("yyyyMMdd") + ")";
                    lmal.RemoveAt(i);
                    //前一个区间已经处理完毕，肯定已经不满足合并条件
                    //将位置i移除后，原i+1变为i，因此不改变索引值继续判断位置i的区间
                    continue;
                }
                i++;
            }
            #endregion

            #region Flat Merge: 合并连续涨幅较小的区间
            #endregion

            #region Mixed Rise Merge: 合并震荡上行、下降的区间
            #endregion
        }
        #endregion

        #region ReviseVertexPosition() 修正区间顶点位置 (长期价格趋势) 
        /// <summary>
        /// 修正区间顶点位置 (长期价格趋势) ，直接修改入参lmal
        /// </summary>
        /// <param name="lmal">输入、输出：长期趋势区间</param>
        /// <param name="lk">K线数据: 列表</param>
        /// <param name="dw">K线数据: 字典</param>
        private void ReviseVertexPosition(List<KTrendMALong> lmal , IList<KJapaneseData> lk, IDictionary<DateTime, KDataWrapper> dw){
            for (int i = 0; i < lmal.Count - 1;){ //对区间截止点进行调整，最后一个区间无需调整
                bool findMax = true, leftFirst = true;
                if (lmal[i].Id <= 0 && lmal[i + 1].Id <= 0){
                    //当前区间、下一区间都属于长期趋势中的区间
                    findMax = this.FindKData(dw, lmal[i].StartDate).MALong < this.FindKData(dw, lmal[i].EndDate).MALong;
                    //优先查找涨速较快的一侧
                    leftFirst = Math.Abs(lmal[i].ChangeSpeed) > Math.Abs(lmal[i + 1].ChangeSpeed);
                    this.ReviseVertexPosition(lmal[i], lmal[i + 1], findMax, leftFirst, lk, dw);
                    i++;
                    continue;
                }
                if (lmal[i].Id <= 0 && lmal[i + 1].Id > 0){
                    //当前区间属于长期趋势中的区间，下一区间属于短期趋势中的区间
                    findMax = lmal[i+1].StartValue > lmal[i+1].EndValue;
                    leftFirst = false;
                    this.ReviseVertexPosition(lmal[i], lmal[i + 1], findMax, leftFirst, lk, dw);
                    i++;
                    continue;
                }
                if (lmal[i].Id > 0){
                    //当前区间属于短期趋势中的区间
                    findMax = lmal[i].StartValue < lmal[i+1].StartValue;
                    leftFirst = true;
                    this.ReviseVertexPosition(lmal[i], lmal[i + 1], findMax, leftFirst, lk, dw);
                    if (i + 2 < lmal.Count && lmal[i+2].Id<=0){
                        findMax = this.FindKData(dw, lmal[i+2].StartDate).MALong < this.FindKData(dw, lmal[i+2].EndDate).MALong;
                        leftFirst = Math.Abs(lmal[i+1].ChangeSpeed) > Math.Abs(lmal[i + 2].ChangeSpeed);
                        this.ReviseVertexPosition(lmal[i+1], lmal[i+2], findMax, leftFirst, lk, dw);
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
        private void ReviseVertexPosition(KTrendMALong cur, KTrendMALong next, bool findMax, bool leftFirst
            , IList<KJapaneseData> lk, IDictionary<DateTime, KDataWrapper> dw){
            int LookAroundDaysL = 5; //长期趋势区间，从前、后n天中查找更佳的顶点位置
            int LookAroundDaysS = 2; //短期趋势区间，从前、后n天中查找更佳的顶点位置
            decimal IgnoreRate = 0.03m; //顶点优先朝涨速较快的一侧调整，如果涨速较慢的一侧差异超过x%，则朝较慢一侧调整

            int lookAroundDays = cur.Id > 0 || next.Id > 0 ? LookAroundDaysS : LookAroundDaysL;
            int index = this.FindKWrapper(dw, cur.EndDate).Index; //区间截止点在K线数据中的索引位置
            int match = index; //比截止点更适合的顶点位置

            int dr = leftFirst ? -1 : 1;
            //优先一侧
            for (int j = 1; j <= lookAroundDays; j++){
                if (index + j * dr < 0 || index + j * dr >= lk.Count
                    || lk[index + j * dr].TxDate <= cur.StartDate || lk[index + j * dr].TxDate >= next.EndDate)
                    break; //越界
                if (findMax){
                    if (lk[index + j * dr].ClosePrice > lk[match].ClosePrice)
                        match = index + j * dr;
                } else{
                    if (lk[index + j * dr].ClosePrice < lk[match].ClosePrice)
                        match = index + j * dr;
                }
            }
            //另外一侧
            dr = dr * -1;
            for (int j = 1; j <= lookAroundDays; j++){
                if (index + j * dr < 0 || index + j * dr >= lk.Count
                    || lk[index + j * dr].TxDate <= cur.StartDate || lk[index + j * dr].TxDate >= next.EndDate)
                    break; //越界
                if (findMax){
                    if (lk[index + j * dr].ClosePrice > lk[match].ClosePrice * (1 + IgnoreRate))
                        match = index + j * dr;
                } else{
                    if (lk[index + j * dr].ClosePrice < lk[match].ClosePrice * (1 - IgnoreRate))
                        match = index + j * dr;
                }
            }
            //fix vertex position
            if (match != index){
                Info("[move-vertex] [" + lk[index].TxDate.ToString("yyMMdd") + " " + lk[index].ClosePrice.ToString("f2")
                    + "] -> [" + lk[match].TxDate.ToString("yyMMdd") + " " + lk[match].ClosePrice.ToString("f2") + "]");
                cur.EndDate = lk[match].TxDate;
                cur.EndValue = lk[match].ClosePrice;
                cur.TxDays = this.FindTxDays(dw, cur.StartDate, cur.EndDate);
                KTrend.CalChangeSpeed(cur);
                cur.NetChange = KTrend.CalNetChange(cur.StartValue, cur.EndValue);
                KTrend.CalHighLowValue(cur, MAType.MALong, lk
                                       , this.FindKWrapper(dw, cur.StartDate).Index, this.FindKWrapper(dw, cur.EndDate).Index);

                next.StartDate = lk[match].TxDate;
                next.StartValue = lk[match].ClosePrice;
                next.TxDays = this.FindTxDays(dw, next.StartDate, next.EndDate);
                KTrend.CalChangeSpeed(next);
                next.NetChange = KTrend.CalNetChange(next.StartValue, next.EndValue);
                KTrend.CalHighLowValue(next, MAType.MALong, lk
                                       , this.FindKWrapper(dw, next.StartDate).Index, this.FindKWrapper(dw, next.EndDate).Index);
            }
        }
        #endregion

        #region Split() 拆分暴涨暴跌区间 (长期价格趋势)
        /// <summary>
        /// 拆分暴涨暴跌区间 (长期价格趋势)，返回拆分后的趋势区间
        /// </summary>
        /// <returns>拆分后的趋势区间</returns>
        /// <param name="lmal">长期趋势区间</param>
        /// <param name="dw">K线数据字典</param>
        /// <param name="lmas">短期趋势区间</param>
        private List<KTrendMALong> Split(List<KTrendMALong> lmal,
            IList<KJapaneseData> lk, IDictionary<DateTime, KDataWrapper> dw, List<KTrendMAShort> lmas){
            //基于短期趋势，查找快速主升和暴跌的区间，将这类区间单独拆分出来，避免他们被前后横盘震荡区间消弱
            List<KTrendMAShort> jumpAndCrash = new List<KTrendMAShort>();
            decimal NCForJumpCrash = 0.25m; //涨跌幅超过多少算暴涨暴跌
            decimal SpeedForJumpCrash = 1m; //涨跌速度至少达到多少才算暴涨暴跌
            int DaysDistForDup = 4;
            decimal NCPropotionOnLongBull = 0.65m;
            decimal SpeedRatioOnLongBull = 2m;

            for(int i=0; i<lmas.Count; i++){
                KTrendMAShort mas = lmas[i];
                if (Math.Abs(KTrend.CalNetChange(mas.StartValue, mas.EndValue)) > NCForJumpCrash 
                    && mas.ChangeSpeed >= SpeedForJumpCrash){
                    bool isDuplicated = false;
                    KTrendMALong container = null;
                    foreach(KTrendMALong mal in lmal){
                        //判断拆分出来的暴涨暴跌区间是否与长期趋势区间重合
                        //如果暴涨暴跌区间的开始日期和结束日期，与长期趋势区间的开始日期和结束日期前后相差不大，则认为是重叠区域，
                        //   这种情况无需对长期趋势区间进行拆分
                        if (!(mas.StartDate >= mal.EndDate || mas.EndDate <= mal.StartDate)){ //排除掉长期趋势和短期趋势区间不可能重叠的情况
                            //开始日期和结束日期相差天数
                            int days1 = this.FindTxDays(dw
                                , mas.StartDate<mal.StartDate ? mas.StartDate : mal.StartDate
                                , mas.StartDate<mal.StartDate ? mal.StartDate : mas.StartDate);
                            int days2 = this.FindTxDays(dw
                                , mas.EndDate<mal.EndDate ? mas.EndDate : mal.EndDate
                                , mas.EndDate<mal.EndDate ? mal.EndDate : mas.EndDate);
                            if (days1 <= DaysDistForDup && days2 <= DaysDistForDup){
                                isDuplicated = true;
                                break;
                            }
                        }
                        //判断暴涨暴跌区间是否被包含在一个长牛趋势中，如果被包含在长牛趋势中，则不将暴涨暴跌区间独立拆分出来
                        //备注：这里的主要目的是将暴涨暴跌区间从前后的横盘震荡趋势中拆分出来
                        if (mas.StartDate >= mal.StartDate && mas.EndDate <= mal.EndDate){
                            if (i >= 2
                                //当前短期趋势和前面2个短期趋势都包含在同一个长期趋势中
                                && lmas[i - 1].StartDate >= mal.StartDate && lmas[i - 1].EndDate <= mal.EndDate
                                && lmas[i - 2].StartDate >= mal.StartDate && lmas[i - 2].EndDate <= mal.EndDate){
                                container = mal;
                                break;
                            }
                            if (i < lmas.Count - 2
                                //当前短期趋势和后面2个短期趋势都包含在同一个长期趋势中
                                && lmas[i + 1].StartDate >= mal.StartDate && lmas[i + 1].EndDate <= mal.EndDate
                                && lmas[i + 2].StartDate >= mal.StartDate && lmas[i + 2].EndDate <= mal.EndDate){
                                container = mal;
                                break;
                            }
                        }
                    }
                    if (isDuplicated) //与长期趋势区间重叠，无需拆分
                        continue; 
                    if (container == null){ //暴涨暴跌区间不在长牛趋势中，执行拆分
                        jumpAndCrash.Add(mas);
                    }else {
                        //进一步判断，匹配出来的长牛区间，是否真是长牛
                        decimal ncLong = KTrend.CalNetChange(container.StartValue, container.EndValue);
                        decimal ncShort = KTrend.CalNetChange(mas.StartValue, mas.EndValue);
                        if (ncShort / ncLong > NCPropotionOnLongBull && mas.ChangeSpeed / container.ChangeSpeed > SpeedRatioOnLongBull){
                            jumpAndCrash.Add(mas);
                        }
                    }
                }
            }
            List<KTrendMALong> result = new List<KTrendMALong> (lmal.Count);
            //Repartition listLong
            for (int i = 0; i < lmal.Count; ){
                if (jumpAndCrash.Count <= 0){
                    result.Add(lmal[i]);
                    i++;
                    continue;
                }
                if (lmal[i].EndDate <= jumpAndCrash[0].StartDate){
                    result.Add(lmal[i]);
                    i++;
                    continue;
                }
                KTrendMALong mal;
                if (lmal[i].StartDate < jumpAndCrash[0].StartDate){
                    mal = new KTrendMALong()
                    {
                        Id = 0,
                        StockId = lmal[i].StockId,
                        StartDate = lmal[i].StartDate,
                        StartValue = lmal[i].StartValue,
                        EndDate = jumpAndCrash[0].StartDate,
                        EndValue = this.FindKData(dw, jumpAndCrash[0].StartDate).ClosePrice,
                        TxDays = this.FindTxDays(dw, lmal[i].StartDate, jumpAndCrash[0].StartDate)
                    };
                    mal.Remark = lmal[i].Remark + "; Trunc(,<<" + lmal[i].EndDate.ToString("yyyyMMdd") + ")";
                    KTrend.CalChangeSpeed(mal);
                    KTrend.CalHighLowValue(mal, MAType.MALong, lk, this.FindKWrapper(dw, mal.StartDate).Index, this.FindKWrapper(dw, mal.EndDate).Index);
                    mal.NetChange = KTrend.CalNetChange(mal.StartValue, mal.EndValue);
                    result.Add(mal);
                }
                if (lmal[i].EndDate < jumpAndCrash[0].EndDate){
                    i++;
                    continue;
                }
                mal = new KTrendMALong()
                {
                    Id = 99,
                    StockId = jumpAndCrash[0].StockId,
                    StartDate = jumpAndCrash[0].StartDate,
                    StartValue = this.FindKData(dw, jumpAndCrash[0].StartDate).ClosePrice,
                    EndDate = jumpAndCrash[0].EndDate,
                    EndValue = this.FindKData(dw, jumpAndCrash[0].EndDate).ClosePrice,
                    TxDays = this.FindTxDays(dw, jumpAndCrash[0].StartDate, jumpAndCrash[0].EndDate)
                };
                mal.Remark = "MAS";
                KTrend.CalChangeSpeed(mal);
                KTrend.CalHighLowValue(mal, MAType.MALong, lk, this.FindKWrapper(dw, mal.StartDate).Index, this.FindKWrapper(dw, mal.EndDate).Index);
                mal.NetChange = KTrend.CalNetChange(mal.StartValue, mal.EndValue);
                Info("[strong-motion] [" + mal.StartDate.ToString("yyMMdd") 
                    + " > " + mal.EndDate.ToString("yyMMdd") + " " + mal.TxDays + " days] [AM:" 
                    + (KTrend.CalNetChange(mal.StartValue, mal.EndValue) * 100).ToString("f2") + "] [speed:" 
                    + mal.ChangeSpeed.ToString("f2") + "]");
                result.Add(mal);
                if (lmal[i].StartDate < mal.EndDate){
                    lmal[i].Remark += "; Trunc(" + lmal[i].StartDate.ToString("yyyyMMdd") + ">>,)";
                    lmal[i].StartDate = mal.EndDate;
                    lmal[i].StartValue = mal.EndValue;
                    lmal[i].TxDays = this.FindTxDays(dw, lmal[i].StartDate, lmal[i].EndDate);
                    KTrend.CalChangeSpeed(lmal[i]);
                    KTrend.CalHighLowValue(lmal[i], MAType.MALong, lk, this.FindKWrapper(dw, lmal[i].StartDate).Index, this.FindKWrapper(dw, lmal[i].EndDate).Index);
                    lmal[i].NetChange = KTrend.CalNetChange(lmal[i].StartValue, lmal[i].EndValue);
                }
                jumpAndCrash.RemoveAt(0);
            }

			return result;
		}
        #endregion

        #region 辅助方法
        private KJapaneseData FindKData(IDictionary<DateTime, KDataWrapper> dw, DateTime date){
            if (dw.ContainsKey(date))
                return dw[date].KData;
            return null;
        }

        private KDataWrapper FindKWrapper(IDictionary<DateTime, KDataWrapper> dw, DateTime date){
            if (dw.ContainsKey(date))
                return dw[date];
            return null;
        }

        private int FindTxDays(IDictionary<DateTime, KDataWrapper> dw, DateTime start, DateTime end){
            KDataWrapper swrapper = this.FindKWrapper(dw, start);
            KDataWrapper ewrapper = this.FindKWrapper(dw, end);
            if (swrapper == null || ewrapper == null)
                return 1;
            return ewrapper.Index - swrapper.Index + 1;
        }

        private class KDataWrapper{
            public KJapaneseData KData { get; set; }
            public int Index { get; set; }
        }
        #endregion
	}
}