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
            Stock stock = Stock.Get(db, stockId);
			IList<KJapaneseData> kdatas = KJapaneseData.Find (db, stockId, start, end);
            IDictionary<int, KTrendMALong> vertexes = new Dictionary<int, KTrendMALong>();
            IList<KTrendMALong> listLong = KTrendMALong.FindAll(db, stockId);
            if (listLong.Count > 0){
                vertexes.Add(int.Parse(listLong[0].StartDate.ToString("yyyyMMdd")), null);
                for (int i = 0; i < listLong.Count; i++){
                    int key = int.Parse(listLong[i].EndDate.ToString("yyyyMMdd"));
                    if (!vertexes.ContainsKey(key))
                        vertexes.Add(key, listLong[i]);
                }
            }
            List<ChartKJapaneseJSON> json = new List<ChartKJapaneseJSON>(kdatas.Count);
			foreach (KJapaneseData d in kdatas) {
                ChartKJapaneseJSON jsonObj = new ChartKJapaneseJSON()
                {
                    d = int.Parse(d.TxDate.ToString("yyyyMMdd")),
                    nc = Convert.ToDecimal(((d.ClosePrice - d.PrevPrice) / d.PrevPrice * 100).ToString("f2")),
                    o = d.OpenPrice,
                    c = d.ClosePrice,
                    hi = d.HighPrice,
                    lo = d.LowPrice,
                    vol = d.Volume,
                    amt = d.Amount,
                    er = stock.CirculatingCapital <= 0 ? 0 
                        : Convert.ToDecimal((d.Volume * 1.0 / stock.CirculatingCapital * 100).ToString("f2")),
                    ms = d.MAShort,
                    ml = d.MALong,
                    vs = d.VMAShort,
                    vl = d.VMALong,
                    vt = 0, vtr = 0, vts = 0, ds = 0
                };
                if (vertexes.ContainsKey(int.Parse(d.TxDate.ToString("yyyyMMdd")))){
                    KTrendMALong malong = vertexes[int.Parse(d.TxDate.ToString("yyyyMMdd"))];
                    if (malong != null){
                        jsonObj.vt = 1;
                        jsonObj.ds = malong.TxDays;
                        jsonObj.vtr = decimal.Parse(((malong.EndValue - malong.StartValue) / malong.StartValue * 100).ToString("f1"));
                        jsonObj.vts = malong.ChangeSpeed;
                    }
                }
                json.Add(jsonObj);
			}
                
            string title = stock.StockCode + " " + stock.StockName 
                + " (" + start.ToString("yyMMdd") + "-" + end.ToString("yyMMdd") + ")";

			string jsonString = JsonConvert.SerializeObject (json, Formatting.Indented);
            string fileName = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar 
                + "html" + Path.DirectorySeparatorChar + "chart-maline.json";
			if(File.Exists(fileName)) File.Delete(fileName);
			File.CreateText(fileName).Close();
            File.WriteAllText(fileName, "var title='" + title + "'; \n var klist = " + jsonString + ";", Encoding.GetEncoding("utf-8"));
		}
	}

    public class ChartKJapaneseJSON{
        public int d { get; set; } //date (yyyyMMdd)
        public decimal nc { get; set; } //涨跌幅 net change
        public decimal o { get; set; } //open price
        public decimal c { get; set; } //close price
        public decimal hi { get; set; } //high price
        public decimal lo { get; set; } //low price
        public long vol { get; set; } //当日成交量 volume
        public long amt { get; set; } //当日成交金额 amount
        public decimal er { get; set; } //当日换手率 exchange rate
        public decimal ms { get; set; } //短期均价 MA short
        public decimal ml { get; set; } //长期均价 MA long
        public long vs { get; set; } //短期均量 VMA short
        public long vl { get; set; } //长期均量 VMA long
        public int vt { get; set; } //是否趋势顶点
        public decimal vtr { get; set; } //趋势区间内涨跌幅（仅趋势顶点有值）
        public decimal vts { get; set; } //趋势区间涨跌速度（仅趋势顶点有值）
        public int ds { get; set; } //趋势区间内交易日天数
    }
}