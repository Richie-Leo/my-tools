using System;
using System.Data;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	public class KTrendMALong : KTrend
	{
		private static string TABLE_NAME = "k_trend_ma_long";
		
		public KTrendMALong() { }
		private KTrendMALong(DataRow row) : base(row) {}
		
		public static int BatchImport(Database db, List<KTrendMALong> entities){
			return BatchImport(db, TABLE_NAME, entities);
		}
		
		public static IList<KTrendMALong> FindAll(Database db, int stoId){
			DataSet ds = KTrend.FindAll(db, TABLE_NAME, stoId);
			IList<KTrendMALong> result = new List<KTrendMALong>(ds.Tables[0].Rows.Count);
			foreach(DataRow row in ds.Tables[0].Rows){
				result.Add(new KTrendMALong(row));
			}
			return result;
		}
	}
}