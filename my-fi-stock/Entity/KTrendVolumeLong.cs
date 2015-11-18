using System;
using System.Data;
using System.Collections.Generic;
using Pandora.Basis.DB;

namespace Pandora.Invest.Entity
{
	public class KTrendVMALong : KTrend
	{
		private static string TABLE_NAME = "k_trend_vma_long";
		
		public KTrendVMALong() { }
		private KTrendVMALong(DataRow row) : base(row) {}
		
		public static int BatchImport(Database db, List<KTrendVMALong> entities){
			return BatchImport(db, TABLE_NAME, entities);
		}
		
		public static IList<KTrendVMALong> FindAll(Database db, int stoId){
			DataSet ds = KTrend.FindAll(db, TABLE_NAME, stoId);
			IList<KTrendVMALong> result = new List<KTrendVMALong>(ds.Tables[0].Rows.Count);
			foreach(DataRow row in ds.Tables[0].Rows){
				result.Add(new KTrendVMALong(row));
			}
			return result;
		}
	}
}