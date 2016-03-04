using System;
using System.Collections.Generic;
using System.Data;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	public partial class Stock
	{
		public static IList<Stock> FindAll(Database db){
			List<Stock> result = new List<Stock>();
			DataSet ds = db.ExecDataSet(string.Format("select * from {0}", Mapper.TableName), null, null);
			if(ds.Tables.Count<=0 || ds.Tables[0].Rows.Count<=0) return result;
			
			
			foreach(DataRow row in ds.Tables[0].Rows){
				result.Add(new Stock(row));
			}
			return result;
		}
		
		public static Stock Get(Database db, string code){
			DataSet ds = db.ExecDataSet(
				string.Format("select * from {0} where {1}=?code", Mapper.TableName, Mapper.StockCode),
				new string[]{"code"}, new object[]{ code }
			);
			if(ds==null || ds.Tables.Count<=0 || ds.Tables[0].Rows.Count<=0) return null;
			return new Stock(ds.Tables[0].Rows[0]);
		}

        public static Stock Get(Database db, int id){
            DataSet ds = db.ExecDataSet(
                string.Format("select * from {0} where {1}=?id", Mapper.TableName, Mapper.StockId),
                new string[]{"id"}, new object[]{ id }
            );
            if(ds==null || ds.Tables.Count<=0 || ds.Tables[0].Rows.Count<=0) return null;
            return new Stock(ds.Tables[0].Rows[0]);
        }
		
		public void UpdateName(Database db){
			db.ExecNonQuery(
				string.Format("update {0} set {1}=?name where {2}=?id", Mapper.TableName, Mapper.StockName, Mapper.StockId)
				, new string[]{"name", "id"}, new object[]{this.StockName, this.StockId});
		}
		
		public void UpdateInfo(Database db){
			db.ExecNonQuery(
				string.Format(
                    "update {0} set {1}=?date,{2}=?tCap,{3}=?cCap,{4}=?eps,{5}=?navps,{6}=?growth,{7}=?loc,{8}=?plat, {9}=?industry where {10}=?id", 
					Mapper.TableName, 
					Mapper.ListDate, Mapper.TotalCapital, Mapper.CirculatingCapital, Mapper.EarningsPerShare,
                    Mapper.NetAssetValuePerShare, Mapper.NetProfitGrowth, Mapper.CompanyLocation, Mapper.Plate, Mapper.Industry,
					Mapper.StockId)
                , new string[]{"date","tCap","cCap","eps","navps","growth","loc","plat","industry","id"}
				, new object[]{
					this.ListDate, this.TotalCapital, this.CirculatingCapital, this.EarningsPerShare,
					this.NetAssetValuePerShare, this.NetProfitGrowth, this.CompanyLocation,
                    this.Plate, this.Industry, this.StockId
				});
		}
		
		public void Create(Database db){
			db.ExecNonQuery(
				string.Format(
					"insert into {0} ({1},{2},{3}) values(?id, ?code, ?name)"
					, Mapper.TableName, Mapper.StockId, Mapper.StockCode, Mapper.StockName
				)
				, new string[] {"id","code","name"}
				, new object[] {this.StockId, this.StockCode, this.StockName});
		}
		
		public bool IsSTStock(){
			return this.StockName.IndexOf("ST", StringComparison.OrdinalIgnoreCase)>=0;
		}
		
		public static IList<Stock> RemoveBlackList(IList<Stock> stocks){
			IList<Stock> r = new List<Stock>(stocks.Count);
			for(int i=0; i<stocks.Count; i++){
				if(stocks[i].StockName.IndexOf("ST", StringComparison.OrdinalIgnoreCase)>=0) continue;
				if(stocks[i].ListDate>DateTime.Now.AddMonths(-2)) continue;
				r.Add(stocks[i]);
			}
			return r;
		}
		
		public override string ToString()
		{
			return this.StockCode;
		}
	}
}
