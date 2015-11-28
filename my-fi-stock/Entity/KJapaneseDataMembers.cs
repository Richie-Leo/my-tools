using System;
using System.Data;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	/// <summary>
	/// 日K线数据
	/// </summary>
	public partial class KJapaneseData
	{
		private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(KJapaneseData));
		public static long _dbTime = 0, _entityTime = 0;

		/// <summary>
		/// 批量导入日K线数据。
		/// 1. klines只能包含1支股票的日K线数据；
		/// 2. klines中的数据必须按照交易日期升序排序；
		/// </summary>
		/// <param name="db"></param>
		/// <param name="kdatas"></param>
		/// <returns></returns>
		public static int BatchImport(Database db, List<KJapaneseData> kdatas){
			if(kdatas==null || kdatas.Count<=0) return 0;
			try{
				int stockId = kdatas[0].StockId;
				if(stockId!=998) return 0; //TODO
				DateTime start = DateTime.Now;
				//因为要计算250日均线，所以向前加载K线数据
				List<KJapaneseData> latest = FindLatest(db, stockId, 251)as List<KJapaneseData>; 

				#region 导入K线数据前的初始化处理
				//1.数据库中已经存在K线数据时，将数据库中最新交易日的K线数据删除，这是为了防止在开盘时间导出的K线数据，
				//  导致当天的K线数据与收盘时不一致，删除重新导入；
				if(latest.Count>0){
					latest[latest.Count-1].Remove(db);
					latest.RemoveAt(latest.Count-1);
				}
				//2.如果数据库中已经存在K线数据，则这些数据不再重新导入；
				while(kdatas.Count>0 && latest.Count>0 && kdatas[0].TxDate<=latest[latest.Count-1].TxDate){
					kdatas.RemoveAt(0);
				}
				//3.如果latest中不足250条数据，则用最早的k线数据补全
				KJapaneseData cloneTarget = null;
				int persistedStart = 0;
				if(latest.Count>0) cloneTarget = latest[0];
				else cloneTarget = kdatas[0];
				while(latest.Count<250){
					latest.Insert(0, cloneTarget.Clone(-1));
					persistedStart++;
				}
				//4.把本次要导入的数据添加到latest中
				int impStart = latest.Count, impEnd = impStart + kdatas.Count - 1;
				//将需要导入的日K线数据kdatas追加到latest后面，计算过程中改变latest中实体的MA均线价格，这些修改
				//同时也会反应到kdatas中的实体上。
				//最后将kdatas插入数据库，即完成数据导入
				latest.AddRange(kdatas);
				//5.在latest最后追加16条最新k线数据的拷贝，用于计算定制化的均价、均量
				cloneTarget = kdatas[kdatas.Count-1];
				long latestEffectiveVol = cloneTarget.Volume;
				for(int i=kdatas.Count-1; i>=0; i--){
					if(!kdatas[i].IsAllDayOnFusingPrice()){
						latestEffectiveVol = kdatas[i].Volume;
						break;
					}
				}
				for(int i=0; i<16; i++){
					KJapaneseData cloned = cloneTarget.Clone(1);
					//1子板涨跌停期间对均量线影响较大，为排除这种影响，克隆对象取最后一个非涨跌停板的成交量
					cloned.Volume = latestEffectiveVol;
					latest.Add(cloned);
				}
				#endregion

				#region 计算标准化MA均线价格
				//初始化各均线价格
				decimal ma5=0, ma10=0, ma20=0, ma60=0, ma120=0, ma250=0;
				for(int i=0; i<impStart; i++){
					ma250 += latest[i].ClosePrice;
					if(impStart-i<=120) ma120 += latest[i].ClosePrice;
					if(impStart-i<=60) ma60 += latest[i].ClosePrice;
					if(impStart-i<=20) ma20 += latest[i].ClosePrice;
					if(impStart-i<=10) ma10 += latest[i].ClosePrice;
					if(impStart-i<=5) ma5 += latest[i].ClosePrice;
				}
				for(int i=impStart; i<=impEnd; i++){
					//设置上一交易日日期及价格
					if(!latest[i-1].IsCloned()){
						latest[i].PrevDate = latest[i-1].TxDate;
						latest[i].PrevPrice = latest[i-1].ClosePrice;
					}
					//5日均线价格
					ma5 = ma5 + latest[i].ClosePrice - latest[i-5].ClosePrice;
					latest[i].MA5 = ma5 / 5;
					//10日均线价格
					ma10 = ma10 + latest[i].ClosePrice - latest[i-10].ClosePrice;
					latest[i].MA10 = ma10 / 10;
					//20日均线价格
					ma20 = ma20 + latest[i].ClosePrice - latest[i-20].ClosePrice;
					latest[i].MA20 = ma20 / 20;
					//60日均线价格
					ma60 = ma60 + latest[i].ClosePrice - latest[i-60].ClosePrice;
					latest[i].MA60 = ma60 / 60;
					//120日均线价格
					ma120 = ma120 + latest[i].ClosePrice - latest[i-120].ClosePrice;
					latest[i].MA120 = ma120 / 120;
					//250日均线价格
					ma250 = ma250 + latest[i].ClosePrice - latest[i-250].ClosePrice;
					latest[i].MA250 = ma250 / 250;
				}
				#endregion
				
				CalculateMA(latest, impStart, impEnd, MAType.MAShort);
				CalculateMA(latest, impStart, impEnd, MAType.MALong);
				CalculateMA(latest, impStart, impEnd, MAType.VMAShort);
				CalculateMA(latest, impStart, impEnd, MAType.VMALong);
				
				DateTime afterCalc = DateTime.Now;
			
				#region 批量导入、更新
				db.BeginTransaction();
				//批量插入
				try{
					BulkInserter<KJapaneseData> bi = new KJapaneseDataBulkInserter<KJapaneseData>(db, 200); //初步验证，一批次插入200条性能最好
					for(int i=impStart; i<=impEnd; i++) bi.Push(latest[i]);
					bi.Flush();
					//更新受影响的双重9日均线价格
					int startIndex = impStart - 8;
					if(startIndex<0) startIndex = 0;
					if(startIndex<persistedStart) startIndex = persistedStart;
					for(int i=startIndex; i<impStart; i++)
						latest[i].UpdateMA(db);
					db.CommitTransaction();
				}catch(Exception exInner){
					db.RollbackTransaction();
					log.Error("导入日K线数据错误", exInner);
				}
				#endregion
				
				return kdatas.Count;
			}catch(Exception ex){
				log.Error("导入日K线错误", ex);
				return 0;
			}
		}
		
		#region 计算定制化的均价、均量
		enum MAType { MAShort = 1, MALong = 2, VMAShort = 3, VMALong = 4 }
		
		private static void CalculateMA(IList<KJapaneseData> impList, int impStart, int impEnd, MAType type){
			//初始化参数
			int n = 0, round = 0, weight = 0;
			switch(type){
				case MAType.MAShort: n = 2; round = 1; weight = 2; break;
				case MAType.MALong: n = 4; round = 2; weight = 1; break;
				case MAType.VMAShort: n = 2; round = 1; weight = 1; break;
				case MAType.VMALong: n = 2; round = 3; weight = 1; break;
			}
			//待计算的节点从impStart到impEnd，因此而受影响、需要重新计算的节点从start到end
			int start = impStart - n*round, end = impEnd;
			
			//初始化计算节点原始值
			//注意：因为计算均量时会忽略一字板涨跌停日期，因此values中元素个数可能小于end-start+1。
			IList<decimal> values = new List<decimal>(end - start + 1 + n*2*round); //前、后多取n个节点，乘以2
			switch(type){
				case MAType.MAShort:
				case MAType.MALong:
					//为了计算start到end的均价、均量值，需前、后追加n*round个k线数据
					for(int i=start-n*round; i<=end+n*round; i++){
						values.Add(impList[i].ClosePrice);
					}
					break;
				case MAType.VMAShort:
				case MAType.VMALong: //忽略一字板涨跌停
					//头部追加部分
					for(int i=impStart-1; i>=0 && values.Count < n*round*2; i--){ //从start开始往前推，尝试找到2*n*round个非1字板数据
						if(!ShouldBeIgnored(impList[i])) values.Insert(0, Convert.ToDecimal(impList[i].Volume));
						if(values.Count<=n*round)
							start = i; //因为忽略1字板涨跌停，不像计算均价情况下开始位置是固定的，这里动态确定开始位置
					}
					//待计算节点部分
					for(int i=impStart; i<=end+n*round; i++){ //尾部通过clone追加的节点一定符合条件，因此可以确保尾部追加了n*round个节点
						if(!ShouldBeIgnored(impList[i])) values.Add(Convert.ToDecimal(impList[i].Volume));
					}
					break;
			}
			
			//进行计算
			//注意：CalcCusMA需要的是计算起始点、终止点在values中的索引，而start、end是在kdatas中的索引，需要转换
			IList<decimal> result = CalculateMA(values, n, values.Count-1-n, n, round, type, weight);
			
			//设置计算结果
			int resultIndex = 0; //计算节点结果值的索引
			for(int i=start; i<=end; i++){
				switch(type){
					case MAType.MAShort:
						impList[i].MAShort = result[resultIndex++]; //注意：计算节点的结果值在result中从索引0开始
						break;
					case MAType.MALong:
						impList[i].MALong = result[resultIndex++];
						break;
					case MAType.VMAShort:
						if(ShouldBeIgnored(impList[i]))
							impList[i].VMAShort = impList[i-1].VMAShort;
						else
							impList[i].VMAShort = Convert.ToInt64(result[resultIndex++]);
						break;
					case MAType.VMALong:
						if(ShouldBeIgnored(impList[i]))
							impList[i].VMALong = impList[i-1].VMALong;
						else
							impList[i].VMALong = Convert.ToInt64(result[resultIndex++]);
						break;
				}
			}
		}
		private static bool ShouldBeIgnored(KJapaneseData e){
			if(e==null) return true;
			if(e.IsCloned()) return false;
			if(e.IsAllDayOnFusingPrice()) return true;
			return false;
		}
		/// <summary>
		/// 
		/// </summary>
		/// <param name="values">值列表，必须保证start前至少有n*round个元素，end后至少有n*round个元素</param>
		/// <param name="start"></param>
		/// <param name="end"></param>
		/// <param name="n"></param>
		/// <param name="round">需要重复计算几轮</param>
		/// <param name="type"></param>
		/// <returns>返回结果均值列表，结果列表中索引0到最后的一个值分别对应values中索引start到end元素的计算结果均值。<br />
		/// 每经过一轮计算，返回的结果列表元素个数比values减少2n个（头、为各减少n个）</returns>
		private static IList<decimal> CalculateMA(IList<decimal> values, int start, int end, int n, int round, MAType type, int weight){
			int s=start, e=end; 
			for(int loop=round; loop>=1; loop--){
				//注意：
				//1.CalcCusMA需要的是计算起始点、终止点在values中的索引，而start、end是在kdatas中的索引，不能使用start和end；
				//2.每经历1次CalcCusMA调用，values中的元素个数减少2n个；
				values = CalculateMA(values, s, e, n, type, weight); //第一次使用入参start、end作为CalcCusMA的start、end
				//执行1次CalcCusMA后，values中前后多余的元素已经被移除，后续再调用CalcCusMA时，参数start、end即变成固定值
				s = n;
				e = values.Count-1-n;
			}
			return values;
		}
		/// <summary>
		/// 计算定制化移动均线值
		/// </summary>
		/// <param name="values">值列表，必须保证start前至少有n个元素，end后至少有n个元素</param>
		/// <param name="start">起始点在values中的索引位置</param>
		/// <param name="end">终止点在values中的索引位置</param>
		/// <param name="n">相对于计算节点，分别取前、后几个节点值计算均值。计算9日均价，n传4。</param>
		/// <param name="type"></param>
		/// <returns>返回结果均值列表，结果列表中索引0到最后的一个值分别对应values中索引start到end元素的计算结果均值。<br />
		/// 每经过一轮计算，返回的结果列表元素个数比values减少2n个（头、为各减少n个）</returns>
		private static IList<decimal> CalculateMA(IList<decimal> values, int start, int end, int n, MAType type, int weight){
			List<decimal> r = new List<decimal>(end - start + 1);
			decimal ma;
			if(weight<=1) weight=1;
			int count = 2*n+1 + weight-1; //参与均值计算的节点个数
			for(int i=start; i<=end; i++){
				switch(type){
				case MAType.MAShort:
				case MAType.MALong:
					//均值
					ma = values [i] * weight;
					for (int j = 1; j <= n; j++) {
						ma = ma + values [i + j] + values [i - j];
					}
					r.Add(ma / count);
					break;
				case MAType.VMAShort:
				case MAType.VMALong:
					//均方根值
					ma = values [i] * values [i] * weight;
					for (int j = 1; j <= n; j++) {
						ma = ma + values [i + j] * values [i + j] + values [i - j] * values [i - j];
					}
					r.Add (Convert.ToDecimal (Math.Sqrt (Convert.ToDouble (ma) / count)));
					break;
				}
			}
			return r;
		}
		#endregion

		private bool Remove(Database db){
			return db.ExecNonQuery(
				string.Format("delete from {0} where {1}=?id", Mapper.TableName, Mapper.Id), 
				new string[] { "id" }, new object[] { this.Id }
			) > 0;
		}
		
		private bool UpdateMA(Database db){
			return db.ExecNonQuery(
				string.Format("update {0} set {2}=?macs, {3}=?macl, {4}=?vmacs, {5}=?vmacl where {1}=?id"
				              , Mapper.TableName, Mapper.Id, Mapper.MAShort, Mapper.MALong, Mapper.VMAShort, Mapper.VMALong),
				new string[] { "id", "macs", "macl", "vmacs", "vmacl" }, 
				new object[] { this.Id, this.MAShort, this.MALong, this.VMAShort, this.VMALong }
			) > 0;
		}
		
		private KJapaneseData Clone(int days){
			return new KJapaneseData(){
				Id = -9999, StockId = this.StockId, TxDate = this.TxDate.AddDays(days),
				OpenPrice = this.OpenPrice, HighPrice = this.HighPrice, LowPrice = this.LowPrice, ClosePrice = this.ClosePrice,
				Volume = this.Volume, Amount = this.Amount
			};
		}
		
		public bool IsCloned(){
			return this.Id==-9999;
		}
		
		/// <summary>
		/// 是否全天位于1字板涨跌停价位
		/// </summary>
		/// <returns></returns>
		public bool IsAllDayOnFusingPrice(){
			return this.OpenPrice>0
				&& this.OpenPrice==this.HighPrice
				&& this.OpenPrice==this.LowPrice
				&& this.OpenPrice==this.ClosePrice;
		}
		
		/// <summary>
		/// 获取最新topRows条K线数据，返回的列表按交易日期<b>【升序】</b>排序。
		/// </summary>
		/// <param name="db"></param>
		/// <param name="stockId"></param>
		/// <param name="topRows"></param>
		/// <returns></returns>
		public static IList<KJapaneseData> FindLatest(Database db, int stockId, int topRows){
			DateTime start = DateTime.Now;
			
			DataSet ds = db.ExecDataSet(
				string.Format("select * from {0} where {1}=?stoId order by {2} desc limit ?rows"
				              , Mapper.TableName, Mapper.StockId, Mapper.TxDate),
				new string[]{"stoId", "rows"}, new object[]{ stockId, topRows }
			);
			
			DateTime loaded = DateTime.Now;
			
			List<KJapaneseData> result = new List<KJapaneseData>(topRows);
			if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
				foreach(DataRow row in ds.Tables[0].Rows){
					result.Add(new KJapaneseData(row));
				}
			}
			result.Reverse();
			
			_dbTime += Convert.ToInt64((loaded - start).TotalMilliseconds);
			_entityTime += Convert.ToInt64((DateTime.Now - loaded).TotalMilliseconds);
			
			return result;
		}
		
		public static IList<KJapaneseData> FindAll(Database db, int stockId){
			DataSet ds = db.ExecDataSet(
				string.Format("select * from {0} where {1}=?stoId"
				              , Mapper.TableName, Mapper.StockId),
				new string[]{"stoId"}, new object[]{ stockId }
			);
			IList<KJapaneseData> result = new List<KJapaneseData>();
			if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
				foreach(DataRow row in ds.Tables[0].Rows){
					result.Add(new KJapaneseData(row));
				}
			}
			return result;
		}

		public static IList<KJapaneseData> Find(Database db, int stockId, DateTime start, DateTime end){
			DataSet ds = db.ExecDataSet(
				string.Format("select * from {0} where {1}=?stoId and {2}>=?start and {2}<=?end order by {2}"
					, Mapper.TableName, Mapper.StockId, Mapper.TxDate),
				new string[]{"stoId", "start", "end"}, new object[]{ stockId, start, end }
			);

			List<KJapaneseData> result = new List<KJapaneseData>(ds.Tables[0].Rows.Count);
			if(ds!=null && ds.Tables.Count>0 && ds.Tables[0].Rows.Count>0){
				foreach(DataRow row in ds.Tables[0].Rows){
					result.Add(new KJapaneseData(row));
				}
			}

			return result;
		}
	}
}
