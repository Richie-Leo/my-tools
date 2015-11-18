using System;
using System.Data;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	public class KTrendMAShort : KTrend
	{
		private static string TABLE_NAME = "k_trend_ma_short";
		
		public KTrendMAShort() { }
		private KTrendMAShort(DataRow row) : base(row) {}
		
		public static int BatchImport(Database db, List<KTrendMAShort> entities){
			return BatchImport(db, TABLE_NAME, entities);
		}
		
		public static IList<KTrendMAShort> FindAll(Database db, int stoId){
			DataSet ds = KTrend.FindAll(db, TABLE_NAME, stoId);
			IList<KTrendMAShort> result = new List<KTrendMAShort>(ds.Tables[0].Rows.Count);
			foreach(DataRow row in ds.Tables[0].Rows){
				result.Add(new KTrendMAShort(row));
			}
			return result;
		}
	}
}