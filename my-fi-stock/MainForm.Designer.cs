/*
 * Created by SharpDevelop.
 * User: richie
 * Date: 2015/4/29
 * Time: 18:07
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
namespace Pandora.Invest
{
	partial class MainForm
	{
		/// <summary>
		/// Designer variable used to keep track of non-visual components.
		/// </summary>
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.Button btnImpTradeList;
		private System.Windows.Forms.TextBox txtTradeListDir;
		private System.Windows.Forms.Label lblTradeList;
		private System.Windows.Forms.Label lblKLine;
		private System.Windows.Forms.FolderBrowserDialog dlgSelDir;
		private System.Windows.Forms.Button btnSelTradeListDir;
		private System.Windows.Forms.Label lblDatabase;
		private System.Windows.Forms.TextBox txtDatabase;
		private System.Windows.Forms.Button btnSelKLineDir;
		private System.Windows.Forms.TextBox txtKLineDir;
		private System.Windows.Forms.Button btnImpKLine;
		private System.Windows.Forms.Button btnUpdateShareholdersNum;
		private System.Windows.Forms.TextBox txtOutput;
		private System.Windows.Forms.TabControl tabMainWindow;
		private System.Windows.Forms.TabPage tabMain;
		private System.Windows.Forms.TabPage tabRuleConf;
		private System.Windows.Forms.TabPage tabDataCapture;
		private System.Windows.Forms.Label lblKLineDir;
		private System.Windows.Forms.Label lblTimeshareTransDir;
		private System.Windows.Forms.OpenFileDialog dlgSelFile;
		private System.Windows.Forms.Button btnKTrend;
		private System.Windows.Forms.TextBox txtStartStockId;
		private System.Windows.Forms.Label lblStartStockId;
		private System.Windows.Forms.ComboBox comboPlate;
		private System.Windows.Forms.Button btnSelPlateFile;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.TextBox txtPlateFile;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Button btnImpPlate;
		private System.Windows.Forms.ProgressBar progressBar;
		private System.Windows.Forms.Label lblProgressInfo;
		private System.Windows.Forms.Timer timer;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label lblProgressRate;
		private System.Windows.Forms.Button btnFilterStock;
		private System.Windows.Forms.Button btnImpStockExtInfo;
		private System.Windows.Forms.Label lblProgressTitle;
		private System.Windows.Forms.TextBox txtStockCode;
		private System.Windows.Forms.TextBox txtEndDate;
		private System.Windows.Forms.TextBox txtStartDate;
		private System.Windows.Forms.Button btnGenChartKTrend;
		
		/// <summary>
		/// Disposes resources used by the form.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing) {
				if (components != null) {
					components.Dispose();
				}
			}
			base.Dispose(disposing);
		}
		
		/// <summary>
		/// This method is required for Windows Forms designer support.
		/// Do not change the method contents inside the source code editor. The Forms designer might
		/// not be able to load this method if it was changed manually.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.dlgSelDir = new System.Windows.Forms.FolderBrowserDialog();
			this.tabMainWindow = new System.Windows.Forms.TabControl();
			this.tabMain = new System.Windows.Forms.TabPage();
			this.txtOutput = new System.Windows.Forms.TextBox();
			this.txtDatabase = new System.Windows.Forms.TextBox();
			this.lblDatabase = new System.Windows.Forms.Label();
			this.tabDataCapture = new System.Windows.Forms.TabPage();
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblProgressRate = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblProgressTitle = new System.Windows.Forms.Label();
			this.lblProgressInfo = new System.Windows.Forms.Label();
			this.btnGenChartKTrend = new System.Windows.Forms.Button();
			this.txtEndDate = new System.Windows.Forms.TextBox();
			this.txtStartDate = new System.Windows.Forms.TextBox();
			this.txtStockCode = new System.Windows.Forms.TextBox();
			this.btnImpStockExtInfo = new System.Windows.Forms.Button();
			this.btnFilterStock = new System.Windows.Forms.Button();
			this.btnImpPlate = new System.Windows.Forms.Button();
			this.comboPlate = new System.Windows.Forms.ComboBox();
			this.btnSelPlateFile = new System.Windows.Forms.Button();
			this.label5 = new System.Windows.Forms.Label();
			this.txtPlateFile = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.txtStartStockId = new System.Windows.Forms.TextBox();
			this.lblStartStockId = new System.Windows.Forms.Label();
			this.btnKTrend = new System.Windows.Forms.Button();
			this.txtKLineDir = new System.Windows.Forms.TextBox();
			this.txtTradeListDir = new System.Windows.Forms.TextBox();
			this.lblTimeshareTransDir = new System.Windows.Forms.Label();
			this.btnUpdateShareholdersNum = new System.Windows.Forms.Button();
			this.btnImpKLine = new System.Windows.Forms.Button();
			this.lblKLine = new System.Windows.Forms.Label();
			this.lblTradeList = new System.Windows.Forms.Label();
			this.btnImpTradeList = new System.Windows.Forms.Button();
			this.btnSelTradeListDir = new System.Windows.Forms.Button();
			this.btnSelKLineDir = new System.Windows.Forms.Button();
			this.lblKLineDir = new System.Windows.Forms.Label();
			this.tabRuleConf = new System.Windows.Forms.TabPage();
			this.dlgSelFile = new System.Windows.Forms.OpenFileDialog();
			this.timer = new System.Windows.Forms.Timer(this.components);
			this.tabMainWindow.SuspendLayout();
			this.tabMain.SuspendLayout();
			this.tabDataCapture.SuspendLayout();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// dlgSelDir
			// 
			this.dlgSelDir.SelectedPath = "D:\\stock";
			// 
			// tabMainWindow
			// 
			this.tabMainWindow.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
			| System.Windows.Forms.AnchorStyles.Left) 
			| System.Windows.Forms.AnchorStyles.Right)));
			this.tabMainWindow.Appearance = System.Windows.Forms.TabAppearance.FlatButtons;
			this.tabMainWindow.Controls.Add(this.tabMain);
			this.tabMainWindow.Controls.Add(this.tabDataCapture);
			this.tabMainWindow.Controls.Add(this.tabRuleConf);
			this.tabMainWindow.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.tabMainWindow.Location = new System.Drawing.Point(0, 1);
			this.tabMainWindow.Margin = new System.Windows.Forms.Padding(0);
			this.tabMainWindow.Name = "tabMainWindow";
			this.tabMainWindow.Padding = new System.Drawing.Point(6, 2);
			this.tabMainWindow.SelectedIndex = 0;
			this.tabMainWindow.Size = new System.Drawing.Size(1319, 432);
			this.tabMainWindow.TabIndex = 0;
			// 
			// tabMain
			// 
			this.tabMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabMain.Controls.Add(this.txtOutput);
			this.tabMain.Controls.Add(this.txtDatabase);
			this.tabMain.Controls.Add(this.lblDatabase);
			this.tabMain.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tabMain.Location = new System.Drawing.Point(4, 35);
			this.tabMain.Margin = new System.Windows.Forms.Padding(0);
			this.tabMain.Name = "tabMain";
			this.tabMain.Size = new System.Drawing.Size(1311, 393);
			this.tabMain.TabIndex = 0;
			this.tabMain.Text = "主窗口";
			this.tabMain.UseVisualStyleBackColor = true;
			// 
			// txtOutput
			// 
			this.txtOutput.Location = new System.Drawing.Point(7, 50);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOutput.Size = new System.Drawing.Size(1295, 334);
			this.txtOutput.TabIndex = 14;
			// 
			// txtDatabase
			// 
			this.txtDatabase.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.txtDatabase.Location = new System.Drawing.Point(116, 5);
			this.txtDatabase.Name = "txtDatabase";
			this.txtDatabase.Size = new System.Drawing.Size(1186, 35);
			this.txtDatabase.TabIndex = 8;
			this.txtDatabase.Text = "Server=127.0.0.1;Database=my_fi_stock;Uid=root;Pwd=dev;charset=utf8;Pooling=true;" +
	"Max Pool Size=20";
			// 
			// lblDatabase
			// 
			this.lblDatabase.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblDatabase.Location = new System.Drawing.Point(2, 8);
			this.lblDatabase.Name = "lblDatabase";
			this.lblDatabase.Size = new System.Drawing.Size(108, 36);
			this.lblDatabase.TabIndex = 7;
			this.lblDatabase.Text = "数据库:";
			this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabDataCapture
			// 
			this.tabDataCapture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabDataCapture.Controls.Add(this.panel1);
			this.tabDataCapture.Controls.Add(this.btnGenChartKTrend);
			this.tabDataCapture.Controls.Add(this.txtEndDate);
			this.tabDataCapture.Controls.Add(this.txtStartDate);
			this.tabDataCapture.Controls.Add(this.txtStockCode);
			this.tabDataCapture.Controls.Add(this.btnImpStockExtInfo);
			this.tabDataCapture.Controls.Add(this.btnFilterStock);
			this.tabDataCapture.Controls.Add(this.btnImpPlate);
			this.tabDataCapture.Controls.Add(this.comboPlate);
			this.tabDataCapture.Controls.Add(this.btnSelPlateFile);
			this.tabDataCapture.Controls.Add(this.label5);
			this.tabDataCapture.Controls.Add(this.txtPlateFile);
			this.tabDataCapture.Controls.Add(this.label4);
			this.tabDataCapture.Controls.Add(this.label3);
			this.tabDataCapture.Controls.Add(this.txtStartStockId);
			this.tabDataCapture.Controls.Add(this.lblStartStockId);
			this.tabDataCapture.Controls.Add(this.btnKTrend);
			this.tabDataCapture.Controls.Add(this.txtKLineDir);
			this.tabDataCapture.Controls.Add(this.txtTradeListDir);
			this.tabDataCapture.Controls.Add(this.lblTimeshareTransDir);
			this.tabDataCapture.Controls.Add(this.btnUpdateShareholdersNum);
			this.tabDataCapture.Controls.Add(this.btnImpKLine);
			this.tabDataCapture.Controls.Add(this.lblKLine);
			this.tabDataCapture.Controls.Add(this.lblTradeList);
			this.tabDataCapture.Controls.Add(this.btnImpTradeList);
			this.tabDataCapture.Controls.Add(this.btnSelTradeListDir);
			this.tabDataCapture.Controls.Add(this.btnSelKLineDir);
			this.tabDataCapture.Controls.Add(this.lblKLineDir);
			this.tabDataCapture.Location = new System.Drawing.Point(4, 35);
			this.tabDataCapture.Margin = new System.Windows.Forms.Padding(0);
			this.tabDataCapture.Name = "tabDataCapture";
			this.tabDataCapture.Size = new System.Drawing.Size(1311, 393);
			this.tabDataCapture.TabIndex = 2;
			this.tabDataCapture.Text = "数据抓取";
			this.tabDataCapture.UseVisualStyleBackColor = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lblProgressRate);
			this.panel1.Controls.Add(this.progressBar);
			this.panel1.Controls.Add(this.lblProgressTitle);
			this.panel1.Controls.Add(this.lblProgressInfo);
			this.panel1.Location = new System.Drawing.Point(9, 294);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(1291, 85);
			this.panel1.TabIndex = 43;
			// 
			// lblProgressRate
			// 
			this.lblProgressRate.CausesValidation = false;
			this.lblProgressRate.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblProgressRate.Location = new System.Drawing.Point(1204, 47);
			this.lblProgressRate.Name = "lblProgressRate";
			this.lblProgressRate.Size = new System.Drawing.Size(78, 35);
			this.lblProgressRate.TabIndex = 39;
			this.lblProgressRate.Text = "0%";
			this.lblProgressRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// progressBar
			// 
			this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.progressBar.ForeColor = System.Drawing.Color.LimeGreen;
			this.progressBar.Location = new System.Drawing.Point(0, 47);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(1189, 38);
			this.progressBar.TabIndex = 37;
			// 
			// lblProgressTitle
			// 
			this.lblProgressTitle.CausesValidation = false;
			this.lblProgressTitle.Location = new System.Drawing.Point(6, 12);
			this.lblProgressTitle.Name = "lblProgressTitle";
			this.lblProgressTitle.Size = new System.Drawing.Size(324, 42);
			this.lblProgressTitle.TabIndex = 40;
			this.lblProgressTitle.Text = "状态栏";
			// 
			// lblProgressInfo
			// 
			this.lblProgressInfo.CausesValidation = false;
			this.lblProgressInfo.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblProgressInfo.Location = new System.Drawing.Point(798, 6);
			this.lblProgressInfo.Name = "lblProgressInfo";
			this.lblProgressInfo.Size = new System.Drawing.Size(458, 42);
			this.lblProgressInfo.TabIndex = 38;
			this.lblProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnGenChartKTrend
			// 
			this.btnGenChartKTrend.Location = new System.Drawing.Point(1091, 50);
			this.btnGenChartKTrend.Name = "btnGenChartKTrend";
			this.btnGenChartKTrend.Size = new System.Drawing.Size(95, 39);
			this.btnGenChartKTrend.TabIndex = 42;
			this.btnGenChartKTrend.Text = "K线图";
			this.btnGenChartKTrend.UseVisualStyleBackColor = true;
			this.btnGenChartKTrend.Click += new System.EventHandler(this.BtnGenChartKTrendClick);
			// 
			// txtEndDate
			// 
			this.txtEndDate.Location = new System.Drawing.Point(985, 50);
			this.txtEndDate.Name = "txtEndDate";
			this.txtEndDate.Size = new System.Drawing.Size(100, 35);
			this.txtEndDate.TabIndex = 41;
			// 
			// txtStartDate
			// 
			this.txtStartDate.Location = new System.Drawing.Point(878, 49);
			this.txtStartDate.Name = "txtStartDate";
			this.txtStartDate.Size = new System.Drawing.Size(100, 35);
			this.txtStartDate.TabIndex = 40;
			// 
			// txtStockCode
			// 
			this.txtStockCode.Location = new System.Drawing.Point(771, 48);
			this.txtStockCode.Name = "txtStockCode";
			this.txtStockCode.Size = new System.Drawing.Size(100, 35);
			this.txtStockCode.TabIndex = 39;
			// 
			// btnImpStockExtInfo
			// 
			this.btnImpStockExtInfo.Location = new System.Drawing.Point(153, 105);
			this.btnImpStockExtInfo.Name = "btnImpStockExtInfo";
			this.btnImpStockExtInfo.Size = new System.Drawing.Size(169, 44);
			this.btnImpStockExtInfo.TabIndex = 38;
			this.btnImpStockExtInfo.Text = "更新基础资料";
			this.btnImpStockExtInfo.UseVisualStyleBackColor = true;
			this.btnImpStockExtInfo.Click += new System.EventHandler(this.BtnImpStockExtInfoClick);
			// 
			// btnFilterStock
			// 
			this.btnFilterStock.Location = new System.Drawing.Point(51, 105);
			this.btnFilterStock.Name = "btnFilterStock";
			this.btnFilterStock.Size = new System.Drawing.Size(83, 44);
			this.btnFilterStock.TabIndex = 37;
			this.btnFilterStock.Text = "选股";
			this.btnFilterStock.UseVisualStyleBackColor = true;
			this.btnFilterStock.Click += new System.EventHandler(this.BtnFilterStockClick);
			// 
			// btnImpPlate
			// 
			this.btnImpPlate.Location = new System.Drawing.Point(638, 217);
			this.btnImpPlate.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpPlate.Name = "btnImpPlate";
			this.btnImpPlate.Size = new System.Drawing.Size(80, 45);
			this.btnImpPlate.TabIndex = 36;
			this.btnImpPlate.Text = "导入";
			this.btnImpPlate.UseVisualStyleBackColor = true;
			this.btnImpPlate.Click += new System.EventHandler(this.BtnImpPlateClick);
			// 
			// comboPlate
			// 
			this.comboPlate.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboPlate.FormattingEnabled = true;
			this.comboPlate.Items.AddRange(new object[] {
			"上证50",
			"沪深300",
			"融资融券"});
			this.comboPlate.Location = new System.Drawing.Point(517, 216);
			this.comboPlate.Name = "comboPlate";
			this.comboPlate.Size = new System.Drawing.Size(93, 32);
			this.comboPlate.TabIndex = 35;
			// 
			// btnSelPlateFile
			// 
			this.btnSelPlateFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelPlateFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelPlateFile.Location = new System.Drawing.Point(411, 217);
			this.btnSelPlateFile.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelPlateFile.Name = "btnSelPlateFile";
			this.btnSelPlateFile.Size = new System.Drawing.Size(39, 39);
			this.btnSelPlateFile.TabIndex = 34;
			this.btnSelPlateFile.Text = "...";
			this.btnSelPlateFile.UseVisualStyleBackColor = true;
			this.btnSelPlateFile.Click += new System.EventHandler(this.BtnSelPlateFileClick);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label5.Location = new System.Drawing.Point(426, 217);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(89, 45);
			this.label5.TabIndex = 33;
			this.label5.Text = "板块:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPlateFile
			// 
			this.txtPlateFile.Location = new System.Drawing.Point(124, 217);
			this.txtPlateFile.Name = "txtPlateFile";
			this.txtPlateFile.Size = new System.Drawing.Size(286, 35);
			this.txtPlateFile.TabIndex = 31;
			this.txtPlateFile.Text = "D:\\stock\\板块";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.Location = new System.Drawing.Point(15, 216);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(111, 40);
			this.label4.TabIndex = 32;
			this.label4.Text = "文件:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(59, 174);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(245, 40);
			this.label3.TabIndex = 30;
			this.label3.Text = "导入板块成分股";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtStartStockId
			// 
			this.txtStartStockId.Location = new System.Drawing.Point(893, 114);
			this.txtStartStockId.MaxLength = 6;
			this.txtStartStockId.Name = "txtStartStockId";
			this.txtStartStockId.Size = new System.Drawing.Size(113, 35);
			this.txtStartStockId.TabIndex = 29;
			this.txtStartStockId.Text = "1";
			// 
			// lblStartStockId
			// 
			this.lblStartStockId.Location = new System.Drawing.Point(770, 117);
			this.lblStartStockId.Name = "lblStartStockId";
			this.lblStartStockId.Size = new System.Drawing.Size(125, 42);
			this.lblStartStockId.TabIndex = 28;
			this.lblStartStockId.Text = "开始位置:";
			// 
			// btnKTrend
			// 
			this.btnKTrend.Location = new System.Drawing.Point(460, 46);
			this.btnKTrend.Name = "btnKTrend";
			this.btnKTrend.Size = new System.Drawing.Size(180, 44);
			this.btnKTrend.TabIndex = 27;
			this.btnKTrend.Text = "计算量价趋势";
			this.btnKTrend.UseVisualStyleBackColor = true;
			this.btnKTrend.Click += new System.EventHandler(this.BtnKTrendClick);
			// 
			// txtKLineDir
			// 
			this.txtKLineDir.Location = new System.Drawing.Point(119, 45);
			this.txtKLineDir.Name = "txtKLineDir";
			this.txtKLineDir.Size = new System.Drawing.Size(203, 35);
			this.txtKLineDir.TabIndex = 16;
			this.txtKLineDir.Text = "D:\\stock\\klines";
			// 
			// txtTradeListDir
			// 
			this.txtTradeListDir.Location = new System.Drawing.Point(851, 217);
			this.txtTradeListDir.Name = "txtTradeListDir";
			this.txtTradeListDir.Size = new System.Drawing.Size(286, 35);
			this.txtTradeListDir.TabIndex = 11;
			this.txtTradeListDir.Text = "D:\\stock\\002241";
			// 
			// lblTimeshareTransDir
			// 
			this.lblTimeshareTransDir.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblTimeshareTransDir.Location = new System.Drawing.Point(764, 216);
			this.lblTimeshareTransDir.Name = "lblTimeshareTransDir";
			this.lblTimeshareTransDir.Size = new System.Drawing.Size(89, 40);
			this.lblTimeshareTransDir.TabIndex = 19;
			this.lblTimeshareTransDir.Text = "目录:";
			this.lblTimeshareTransDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnUpdateShareholdersNum
			// 
			this.btnUpdateShareholdersNum.Location = new System.Drawing.Point(1012, 115);
			this.btnUpdateShareholdersNum.Name = "btnUpdateShareholdersNum";
			this.btnUpdateShareholdersNum.Size = new System.Drawing.Size(150, 44);
			this.btnUpdateShareholdersNum.TabIndex = 18;
			this.btnUpdateShareholdersNum.Text = "抓取股东数";
			this.btnUpdateShareholdersNum.UseVisualStyleBackColor = true;
			this.btnUpdateShareholdersNum.Click += new System.EventHandler(this.BtnUpdateShareholdersNumClick);
			// 
			// btnImpKLine
			// 
			this.btnImpKLine.Location = new System.Drawing.Point(373, 45);
			this.btnImpKLine.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpKLine.Name = "btnImpKLine";
			this.btnImpKLine.Size = new System.Drawing.Size(79, 45);
			this.btnImpKLine.TabIndex = 15;
			this.btnImpKLine.Text = "导入";
			this.btnImpKLine.UseVisualStyleBackColor = true;
			this.btnImpKLine.Click += new System.EventHandler(this.BtnImpKLineClick);
			// 
			// lblKLine
			// 
			this.lblKLine.Location = new System.Drawing.Point(44, 8);
			this.lblKLine.Name = "lblKLine";
			this.lblKLine.Size = new System.Drawing.Size(201, 39);
			this.lblKLine.TabIndex = 13;
			this.lblKLine.Text = "导入日K线数据";
			this.lblKLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTradeList
			// 
			this.lblTradeList.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblTradeList.Location = new System.Drawing.Point(786, 180);
			this.lblTradeList.Margin = new System.Windows.Forms.Padding(0);
			this.lblTradeList.Name = "lblTradeList";
			this.lblTradeList.Size = new System.Drawing.Size(245, 36);
			this.lblTradeList.TabIndex = 12;
			this.lblTradeList.Text = "导入分时交易明细";
			this.lblTradeList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnImpTradeList
			// 
			this.btnImpTradeList.Location = new System.Drawing.Point(1185, 217);
			this.btnImpTradeList.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpTradeList.Name = "btnImpTradeList";
			this.btnImpTradeList.Size = new System.Drawing.Size(80, 45);
			this.btnImpTradeList.TabIndex = 10;
			this.btnImpTradeList.Text = "导入";
			this.btnImpTradeList.UseVisualStyleBackColor = true;
			this.btnImpTradeList.Click += new System.EventHandler(this.BtnImpTradeListClick);
			// 
			// btnSelTradeListDir
			// 
			this.btnSelTradeListDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelTradeListDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelTradeListDir.Location = new System.Drawing.Point(1136, 217);
			this.btnSelTradeListDir.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelTradeListDir.Name = "btnSelTradeListDir";
			this.btnSelTradeListDir.Size = new System.Drawing.Size(45, 39);
			this.btnSelTradeListDir.TabIndex = 14;
			this.btnSelTradeListDir.Text = "...";
			this.btnSelTradeListDir.UseVisualStyleBackColor = true;
			this.btnSelTradeListDir.Click += new System.EventHandler(this.BtnSelTradeListDirClick);
			// 
			// btnSelKLineDir
			// 
			this.btnSelKLineDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelKLineDir.Location = new System.Drawing.Point(325, 45);
			this.btnSelKLineDir.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelKLineDir.Name = "btnSelKLineDir";
			this.btnSelKLineDir.Size = new System.Drawing.Size(41, 45);
			this.btnSelKLineDir.TabIndex = 17;
			this.btnSelKLineDir.Text = "...";
			this.btnSelKLineDir.UseVisualStyleBackColor = true;
			this.btnSelKLineDir.Click += new System.EventHandler(this.BtnSelKLineDirClick);
			// 
			// lblKLineDir
			// 
			this.lblKLineDir.Font = new System.Drawing.Font("宋体", 9.047121F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblKLineDir.Location = new System.Drawing.Point(2, 45);
			this.lblKLineDir.Name = "lblKLineDir";
			this.lblKLineDir.Size = new System.Drawing.Size(109, 39);
			this.lblKLineDir.TabIndex = 20;
			this.lblKLineDir.Text = "目录:";
			this.lblKLineDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabRuleConf
			// 
			this.tabRuleConf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabRuleConf.Location = new System.Drawing.Point(4, 35);
			this.tabRuleConf.Margin = new System.Windows.Forms.Padding(0);
			this.tabRuleConf.Name = "tabRuleConf";
			this.tabRuleConf.Size = new System.Drawing.Size(1311, 393);
			this.tabRuleConf.TabIndex = 1;
			this.tabRuleConf.Text = "筛选规则设置";
			// 
			// dlgSelFile
			// 
			this.dlgSelFile.Multiselect = true;
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(1320, 442);
			this.Controls.Add(this.tabMainWindow);
			this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "My Stock Tools";
			this.Load += new System.EventHandler(this.MainFormLoad);
			this.tabMainWindow.ResumeLayout(false);
			this.tabMain.ResumeLayout(false);
			this.tabMain.PerformLayout();
			this.tabDataCapture.ResumeLayout(false);
			this.tabDataCapture.PerformLayout();
			this.panel1.ResumeLayout(false);
			this.ResumeLayout(false);

		}
	}
}
