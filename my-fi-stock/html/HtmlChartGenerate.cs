using System;
using System.Text;
using System.Collections.Generic;
using System.IO;

using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using Newtonsoft.Json;

namespace Pandora.Invest.Html
{
	public class HtmlChartGenerate{
		public static void GenerateMALineChart(Database db, int stockId, DateTime start, DateTime end){
			IList<KJapaneseData> kdatas = KJapaneseData.Find (db, stockId, start, end);
            IDictionary<int, decimal> vertexes = new Dictionary<int, decimal>();
            IList<KTrendMALong> listLong = KTrendMALong.FindAll(db, stockId);
            if (listLong.Count > 0){
                vertexes.Add(int.Parse(listLong[0].StartDate.ToString("yyyyMMdd")), listLong[0].StartValue);
                for (int i = 0; i < listLong.Count; i++){
                    int key = int.Parse(listLong[i].EndDate.ToString("yyyyMMdd"));
                    if (!vertexes.ContainsKey(key))
                        vertexes.Add(key, listLong[i].EndValue);
                }
            }
            List<ChartKJapaneseJSON> json = new List<ChartKJapaneseJSON>(kdatas.Count);
			foreach (KJapaneseData d in kdatas) {
                json.Add(new ChartKJapaneseJSON(){
                    d = int.Parse(d.TxDate.ToString("yyyyMMdd")),
                    o = d.PriceOpen,
                    c = d.PriceClose,
                    mi = d.PriceMin,
                    ma = d.PriceMax,
                    ms = d.MACusShort,
                    ml = d.MACusLong,
                    vol = d.Volume,
                    vs = d.VMACusShort,
                    vl = d.VMACusLong,
                    vt = vertexes.ContainsKey(int.Parse(d.TxDate.ToString("yyyyMMdd"))) ? 1 : 0
                });
			}
			string jsonString = JsonConvert.SerializeObject (json, Formatting.Indented);
            string fileName = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar 
                + "html" + Path.DirectorySeparatorChar + "chart-maline.json";
			if(File.Exists(fileName)) File.Delete(fileName);
			File.CreateText(fileName).Close();
			File.WriteAllText(fileName, "var klist = " + jsonString + ";", Encoding.GetEncoding("utf-8"));
		}
	}

    public class ChartKJapaneseJSON{
        public int d { get; set; }
        public decimal o { get; set; }
        public decimal c { get; set; }
        public decimal mi { get; set; }
        public decimal ma { get; set; }
        public decimal ms { get; set; }
        public decimal ml { get; set; }
        public long vol { get; set; }
        public long vs { get; set; }
        public long vl { get; set; }
        public int vt { get; set; }
    }
}