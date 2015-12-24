using System;
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Collections.Generic;

using Pandora.Invest.DataCapture;
using Pandora.Invest.DataCapture.Http;
using Pandora.Basis.DB;
using Pandora.Invest.Entity;
using Pandora.Invest.PickingStrategy;
using Pandora.Invest.MThread;

namespace Pandora.Invest
{
	public partial class MainForm : Form
	{
		private const int THREAD_COUNT = 4;
		
		private ProgressController _progressController;
		
		#region 窗体初始化
		public MainForm()
		{
			log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("log4net.config"));
			InitializeComponent();
			System.Threading.Thread.CurrentThread.Name = "main";
		}
		void MainFormLoad(object sender, EventArgs e)
		{
            this.txtStockCode.Text = "998";
			this.txtStartDate.Text = "2013-01-01";
			this.txtEndDate.Text = DateTime.Now.ToString("yyyy-MM-dd");
			this._progressController = new ProgressController(
				this.timer, this.progressBar, this.lblProgressTitle, this.lblProgressInfo, this.lblProgressRate);
			//校验
			//string message = "开始自检...\r\n";
			//Database db = null;
			//try{
			//	db = new Database(this.txtDatabase.Text);
			//	db.Open();
			//	message = message + "数据库连接 正常！\r\n";
			//}catch(Exception ex){
			//	message = message + "数据库连接异常：" + ex.Message + "\r\n";
			//}
			//db.Close();
			//this.txtOutput.Text = message;
		}
		#endregion
		
		#region 导入日K线数据
		/// <summary>
		/// 从文件导入日K线数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void BtnImpKLineClick(object sender, EventArgs e)
		{
			ProgressStatus status = new ProgressStatus();
			this._progressController.Start("从文件导入日K线", status);
			var tm = new MThreadManager<string>(THREAD_COUNT, typeof(ImpKJapaneseWorker), status);
			tm.SetContext("connection-string", this.txtDatabase.Text)
				.AddItem(Directory.GetFiles(this.txtKLineDir.Text))
				.Start();
		}
		#endregion
		
		void BtnImpStockExtInfoClick(object sender, EventArgs e)
		{
            Database db = new Database(this.txtDatabase.Text);
            db.Open();
            IList<Stock> stocks = Stock.FindAll(db);
            db.Close();

            ProgressStatus status = new ProgressStatus();
            this._progressController.Start("抓取股票基础信息", status);
            var tm = new MThreadManager<Stock>(THREAD_COUNT, typeof(ImpStockInfoWorker), status);
            tm.SetContext("connection-string", this.txtDatabase.Text)
                .AddItem(stocks)
                .Start();
		}
		
		void BtnFilterStockClick(object sender, EventArgs e)
		{
			Database db = new Database(this.txtDatabase.Text);
			db.Open();
			IList<Stock> stocks = Stock.FindAll(db);
			db.Close();
			
			string corssStar = "AM-Open-Close=0.006; AM-Min-Max=0.05; AM-Prev=0.015; AM-Matched-Days=0.03; RatioVolReduce=50; Min-Matched-Days=4; Starting-Point=99999; Regression=true";
			IList<StrategyConfig> confList = new List<StrategyConfig>();
			confList.Add(new StrategyConfig("MicroPriceTrendStrategy", corssStar));
			
			ProgressStatus status = new ProgressStatus();
			this._progressController.Start("执行策略选股...", status);
			var tm = new MThreadManager<Stock>(THREAD_COUNT, typeof(PickingStrategyWorker), status);
			tm.SetContext("connection-string", this.txtDatabase.Text)
				.SetContext("executors", confList)
				.AddItem(stocks)
				.Start();
		}
		
		void BtnKTrendClick(object sender, EventArgs e)
		{
			Database db = new Database(this.txtDatabase.Text);
			db.Open();
			IList<Stock> stocks = Stock.RemoveBlackList(Stock.FindAll(db));
			db.Close();
			
			ProgressStatus status = new ProgressStatus();
			this._progressController.Start("计算K线趋势", status);
			var tm = new MThreadManager<Stock>(THREAD_COUNT, typeof(CalKTrendWorker), status);
			tm.SetContext("connection-string", this.txtDatabase.Text)
				.AddItem(stocks)
				.Start();
		}
		
        void BtnUpdateShareholdersNumClick(object sender, EventArgs e)
        {
            int startId = 1;
            int.TryParse(this.txtStartStockId.Text, out startId);

            Database db = new Database(this.txtDatabase.Text);
            db.Open();
            IList<Stock> stocks = Stock.FindAll(db);
            db.Close();
            while (startId > 1 && stocks.Count > 0 && stocks[0].StockId < startId){
                stocks.RemoveAt(0);
            }

            ProgressStatus status = new ProgressStatus();
            this._progressController.Start("抓取最新股东数", status);
            var tm = new MThreadManager<Stock>(THREAD_COUNT, typeof(ImpShareHoldersNumWorker), status);
            tm.SetContext("connection-string", this.txtDatabase.Text)
                .AddItem(stocks)
                .Start();
        }
		
		
		
		
		#region 
		void BtnSelTradeListDirClick(object sender, EventArgs e)
		{
			if(this.txtTradeListDir.Text.Trim().Length>0)
				dlgSelDir.SelectedPath = this.txtTradeListDir.Text.Trim();
			if(dlgSelDir.ShowDialog() == DialogResult.OK){
				this.txtTradeListDir.Text = this.dlgSelDir.SelectedPath;
			}
		}
		
		void BtnSelKLineDirClick(object sender, EventArgs e)
		{
			if(this.txtKLineDir.Text.Trim().Length>0)
				dlgSelDir.SelectedPath = this.txtKLineDir.Text.Trim();
			if(dlgSelDir.ShowDialog() == DialogResult.OK){
				this.txtKLineDir.Text = this.dlgSelDir.SelectedPath;
			}
		}
		
		/// <summary>
		/// 从文件导入分时交易数据
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		void BtnImpTradeListClick(object sender, EventArgs e)
		{
			Database db = new Database(this.txtDatabase.Text);
			db.Open();
			ImportFileTimeShareTransaction.DoImportDirectory(db, this.txtTradeListDir.Text);
			db.Close();
		}
		
		void BtnSelPlateFileClick(object sender, EventArgs e)
		{
			this.dlgSelFile.Multiselect = false;
			if(this.txtPlateFile.Text.Trim().Length>0){
				this.dlgSelFile.InitialDirectory = this.txtPlateFile.Text;
			}
			if(dlgSelFile.ShowDialog() == DialogResult.OK){
				this.txtPlateFile.Text = this.dlgSelFile.FileName;
			}
		}
		void BtnImpPlateClick(object sender, EventArgs e)
		{
			if(this.txtPlateFile.Text.Trim().Length<=0) return;
			if(!File.Exists(this.txtPlateFile.Text)) return;
			
			string plate = (string)this.comboPlate.SelectedItem;
			if(String.IsNullOrEmpty(plate)) return;
			int plateId = 0;
			switch(plate){
				case "上证50": plateId = 3; break;
				case "沪深300": plateId = 1; break;
				case "融资融券": plateId = 2; break;
			}
			if(plateId<=0) return;
			
			Database db = new Database(this.txtDatabase.Text);
			db.Open();
			
			db.ExecNonQuery("delete from bas_plate_stocks where plate_id=?id"
			                , new string[] { "id" }, new object[] { plateId });
			
			using(StreamReader sr = new StreamReader(this.txtPlateFile.Text, Encoding.Default)){
				string line = "";
				while((line = sr.ReadLine()) != null){
					if(string.IsNullOrEmpty(line)) continue;
					string[] fields = line.Trim().Split('\t');
					if(fields.Length<2 || fields[0].Trim().Length!=6 ||
					   (fields[0].Trim()[0]!= '6' && fields[0].Trim()[0]!= '3' && fields[0].Trim()[0]!= '0')) 
						continue;
					string code = fields[0].Trim();
					int stockId = int.Parse(code);
					if(stockId<=0) continue;
					db.ExecNonQuery("insert into bas_plate_stocks(plate_id, sto_id) values(?pid, ?sid)"
					                , new string[] {"pid", "sid"}, new object[] {plateId, stockId} );
				}
			}
			db.Close();
			
			MessageBox.Show("导入成功", "导入成功"); 
		}
		void BtnGenChartKTrendClick(object sender, EventArgs e)
		{
			Database db = new Database(this.txtDatabase.Text);
			db.Open();
			Pandora.Invest.Html.HtmlChartGenerate.GenerateMALineChart (db, 
				int.Parse (this.txtStockCode.Text), 
				DateTime.Parse (this.txtStartDate.Text), 
				DateTime.Parse (this.txtEndDate.Text)
			);
			db.Close();
		}
		#endregion
	}
}