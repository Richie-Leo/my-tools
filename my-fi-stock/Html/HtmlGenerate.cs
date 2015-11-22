using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Pandora.Basis.DB;
using Pandora.Invest.Entity;

namespace Pandora.Invest.Html
{
	public class HtmlChartGenerate
	{
		public static void GenerateKTrendChart(Database db, int stockId, DateTime start, DateTime end){
			IList<KJapaneseData> kdatas = KJapaneseData.Find (db, stockId, start, end);
			KTrendJSON json = new KTrendJSON (){ 
				dates = new List<string>(), 
				k = new List<decimal[]>(),
				macs = new List<decimal>(),
				macl = new List<decimal>(),
				vol = new List<long>(),
				vmacs = new List<long>(),
				vmacl = new List<long>()
			};
			foreach (KJapaneseData d in kdatas) {
				json.dates.Add (d.TxDate.ToString("yyMMdd"));
				json.k.Add (new decimal[] { d.PriceOpen, d.PriceClose, d.PriceMin, d.PriceMax });
				json.macs.Add (d.MACusShort);
				json.macl.Add (d.MACusLong);
				json.vol.Add (d.Volume);
				json.vmacs.Add (d.VMACusShort);
				json.vmacl.Add (d.VMACusLong);
			}
			string jsonString = Newtonsoft.Json.JsonConvert.SerializeObject (json, Newtonsoft.Json.Formatting.Indented);
			string fileName = "ktrend-chart.json";
			if(File.Exists(fileName)) File.Delete(fileName);
			File.CreateText(fileName).Close();
			File.WriteAllText(fileName, "var chartData = " + jsonString + ";", Encoding.GetEncoding("utf-8"));
		}
	}

	public class KTrendJSON{
		public IList<string> dates { get; set; }
		public IList<decimal[]> k { get; set; }
		public IList<decimal> macs { get; set; }
		public IList<decimal> macl { get; set; }
		public IList<long> vol { get; set; }
		public IList<long> vmacs { get; set; }
		public IList<long> vmacl { get; set; }
	}
}