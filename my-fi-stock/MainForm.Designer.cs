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
			this.panel1 = new System.Windows.Forms.Panel();
			this.lblProgressTitle = new System.Windows.Forms.Label();
			this.lblProgressInfo = new System.Windows.Forms.Label();
			this.progressBar = new System.Windows.Forms.ProgressBar();
			this.lblProgressRate = new System.Windows.Forms.Label();
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
			this.tabMainWindow.Location = new System.Drawing.Point(0, 1);
			this.tabMainWindow.Margin = new System.Windows.Forms.Padding(0);
			this.tabMainWindow.Name = "tabMainWindow";
			this.tabMainWindow.Padding = new System.Drawing.Point(6, 2);
			this.tabMainWindow.SelectedIndex = 0;
			this.tabMainWindow.Size = new System.Drawing.Size(865, 471);
			this.tabMainWindow.TabIndex = 0;
			// 
			// tabMain
			// 
			this.tabMain.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabMain.Controls.Add(this.txtOutput);
			this.tabMain.Controls.Add(this.txtDatabase);
			this.tabMain.Controls.Add(this.lblDatabase);
			this.tabMain.ForeColor = System.Drawing.SystemColors.ControlText;
			this.tabMain.Location = new System.Drawing.Point(4, 27);
			this.tabMain.Margin = new System.Windows.Forms.Padding(0);
			this.tabMain.Name = "tabMain";
			this.tabMain.Size = new System.Drawing.Size(857, 440);
			this.tabMain.TabIndex = 0;
			this.tabMain.Text = "主窗口";
			this.tabMain.UseVisualStyleBackColor = true;
			// 
			// txtOutput
			// 
			this.txtOutput.Location = new System.Drawing.Point(7, 34);
			this.txtOutput.Multiline = true;
			this.txtOutput.Name = "txtOutput";
			this.txtOutput.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.txtOutput.Size = new System.Drawing.Size(849, 399);
			this.txtOutput.TabIndex = 14;
			// 
			// txtDatabase
			// 
			this.txtDatabase.Location = new System.Drawing.Point(58, 5);
			this.txtDatabase.Name = "txtDatabase";
			this.txtDatabase.Size = new System.Drawing.Size(798, 23);
			this.txtDatabase.TabIndex = 8;
			this.txtDatabase.Text = "Server=127.0.0.1;Database=my_fi_stock;Uid=root;Pwd=dev;charset=utf8;Pooling=true;" +
	"Max Pool Size=20";
			// 
			// lblDatabase
			// 
			this.lblDatabase.Location = new System.Drawing.Point(2, 8);
			this.lblDatabase.Name = "lblDatabase";
			this.lblDatabase.Size = new System.Drawing.Size(50, 19);
			this.lblDatabase.TabIndex = 7;
			this.lblDatabase.Text = "数据库:";
			this.lblDatabase.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabDataCapture
			// 
			this.tabDataCapture.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
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
			this.tabDataCapture.Location = new System.Drawing.Point(4, 23);
			this.tabDataCapture.Margin = new System.Windows.Forms.Padding(0);
			this.tabDataCapture.Name = "tabDataCapture";
			this.tabDataCapture.Size = new System.Drawing.Size(857, 444);
			this.tabDataCapture.TabIndex = 2;
			this.tabDataCapture.Text = "数据抓取";
			this.tabDataCapture.UseVisualStyleBackColor = true;
			// 
			// btnImpStockExtInfo
			// 
			this.btnImpStockExtInfo.Location = new System.Drawing.Point(115, 117);
			this.btnImpStockExtInfo.Name = "btnImpStockExtInfo";
			this.btnImpStockExtInfo.Size = new System.Drawing.Size(91, 23);
			this.btnImpStockExtInfo.TabIndex = 38;
			this.btnImpStockExtInfo.Text = "更新基础资料";
			this.btnImpStockExtInfo.UseVisualStyleBackColor = true;
			this.btnImpStockExtInfo.Click += new System.EventHandler(this.BtnImpStockExtInfoClick);
			// 
			// btnFilterStock
			// 
			this.btnFilterStock.Location = new System.Drawing.Point(21, 117);
			this.btnFilterStock.Name = "btnFilterStock";
			this.btnFilterStock.Size = new System.Drawing.Size(75, 23);
			this.btnFilterStock.TabIndex = 37;
			this.btnFilterStock.Text = "选股";
			this.btnFilterStock.UseVisualStyleBackColor = true;
			this.btnFilterStock.Click += new System.EventHandler(this.BtnFilterStockClick);
			// 
			// btnImpPlate
			// 
			this.btnImpPlate.Location = new System.Drawing.Point(784, 352);
			this.btnImpPlate.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpPlate.Name = "btnImpPlate";
			this.btnImpPlate.Size = new System.Drawing.Size(57, 24);
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
			this.comboPlate.Location = new System.Drawing.Point(684, 351);
			this.comboPlate.Name = "comboPlate";
			this.comboPlate.Size = new System.Drawing.Size(93, 25);
			this.comboPlate.TabIndex = 35;
			// 
			// btnSelPlateFile
			// 
			this.btnSelPlateFile.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelPlateFile.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelPlateFile.Location = new System.Drawing.Point(614, 352);
			this.btnSelPlateFile.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelPlateFile.Name = "btnSelPlateFile";
			this.btnSelPlateFile.Size = new System.Drawing.Size(24, 24);
			this.btnSelPlateFile.TabIndex = 34;
			this.btnSelPlateFile.Text = "...";
			this.btnSelPlateFile.UseVisualStyleBackColor = true;
			this.btnSelPlateFile.Click += new System.EventHandler(this.BtnSelPlateFileClick);
			// 
			// label5
			// 
			this.label5.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label5.Location = new System.Drawing.Point(646, 352);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(36, 23);
			this.label5.TabIndex = 33;
			this.label5.Text = "板块:";
			this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// txtPlateFile
			// 
			this.txtPlateFile.Location = new System.Drawing.Point(327, 352);
			this.txtPlateFile.Name = "txtPlateFile";
			this.txtPlateFile.Size = new System.Drawing.Size(286, 23);
			this.txtPlateFile.TabIndex = 31;
			this.txtPlateFile.Text = "D:\\stock\\板块";
			// 
			// label4
			// 
			this.label4.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label4.Location = new System.Drawing.Point(293, 351);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(36, 23);
			this.label4.TabIndex = 32;
			this.label4.Text = "文件:";
			this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// label3
			// 
			this.label3.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.label3.Location = new System.Drawing.Point(294, 330);
			this.label3.Margin = new System.Windows.Forms.Padding(0);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(245, 19);
			this.label3.TabIndex = 30;
			this.label3.Text = "导入板块成分股";
			this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// txtStartStockId
			// 
			this.txtStartStockId.Location = new System.Drawing.Point(679, 236);
			this.txtStartStockId.MaxLength = 6;
			this.txtStartStockId.Name = "txtStartStockId";
			this.txtStartStockId.Size = new System.Drawing.Size(74, 23);
			this.txtStartStockId.TabIndex = 29;
			this.txtStartStockId.Text = "1";
			// 
			// lblStartStockId
			// 
			this.lblStartStockId.Location = new System.Drawing.Point(621, 239);
			this.lblStartStockId.Name = "lblStartStockId";
			this.lblStartStockId.Size = new System.Drawing.Size(60, 23);
			this.lblStartStockId.TabIndex = 28;
			this.lblStartStockId.Text = "开始位置:";
			// 
			// btnKTrend
			// 
			this.btnKTrend.Location = new System.Drawing.Point(327, 23);
			this.btnKTrend.Name = "btnKTrend";
			this.btnKTrend.Size = new System.Drawing.Size(96, 23);
			this.btnKTrend.TabIndex = 27;
			this.btnKTrend.Text = "计算量价趋势";
			this.btnKTrend.UseVisualStyleBackColor = true;
			this.btnKTrend.Click += new System.EventHandler(this.BtnKTrendClick);
			// 
			// txtKLineDir
			// 
			this.txtKLineDir.Location = new System.Drawing.Point(36, 22);
			this.txtKLineDir.Name = "txtKLineDir";
			this.txtKLineDir.Size = new System.Drawing.Size(203, 23);
			this.txtKLineDir.TabIndex = 16;
			this.txtKLineDir.Text = "D:\\stock\\klines";
			// 
			// txtTradeListDir
			// 
			this.txtTradeListDir.Location = new System.Drawing.Point(473, 405);
			this.txtTradeListDir.Name = "txtTradeListDir";
			this.txtTradeListDir.Size = new System.Drawing.Size(286, 23);
			this.txtTradeListDir.TabIndex = 11;
			this.txtTradeListDir.Text = "D:\\stock\\002241";
			// 
			// lblTimeshareTransDir
			// 
			this.lblTimeshareTransDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblTimeshareTransDir.Location = new System.Drawing.Point(439, 404);
			this.lblTimeshareTransDir.Name = "lblTimeshareTransDir";
			this.lblTimeshareTransDir.Size = new System.Drawing.Size(36, 23);
			this.lblTimeshareTransDir.TabIndex = 19;
			this.lblTimeshareTransDir.Text = "目录:";
			this.lblTimeshareTransDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// btnUpdateShareholdersNum
			// 
			this.btnUpdateShareholdersNum.Location = new System.Drawing.Point(759, 237);
			this.btnUpdateShareholdersNum.Name = "btnUpdateShareholdersNum";
			this.btnUpdateShareholdersNum.Size = new System.Drawing.Size(82, 23);
			this.btnUpdateShareholdersNum.TabIndex = 18;
			this.btnUpdateShareholdersNum.Text = "抓取股东数";
			this.btnUpdateShareholdersNum.UseVisualStyleBackColor = true;
			this.btnUpdateShareholdersNum.Click += new System.EventHandler(this.BtnUpdateShareholdersNumClick);
			// 
			// btnImpKLine
			// 
			this.btnImpKLine.Location = new System.Drawing.Point(271, 22);
			this.btnImpKLine.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpKLine.Name = "btnImpKLine";
			this.btnImpKLine.Size = new System.Drawing.Size(42, 24);
			this.btnImpKLine.TabIndex = 15;
			this.btnImpKLine.Text = "导入";
			this.btnImpKLine.UseVisualStyleBackColor = true;
			this.btnImpKLine.Click += new System.EventHandler(this.BtnImpKLineClick);
			// 
			// lblKLine
			// 
			this.lblKLine.Location = new System.Drawing.Point(4, 1);
			this.lblKLine.Name = "lblKLine";
			this.lblKLine.Size = new System.Drawing.Size(123, 23);
			this.lblKLine.TabIndex = 13;
			this.lblKLine.Text = "导入日K线数据";
			this.lblKLine.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// lblTradeList
			// 
			this.lblTradeList.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblTradeList.Location = new System.Drawing.Point(440, 383);
			this.lblTradeList.Margin = new System.Windows.Forms.Padding(0);
			this.lblTradeList.Name = "lblTradeList";
			this.lblTradeList.Size = new System.Drawing.Size(245, 19);
			this.lblTradeList.TabIndex = 12;
			this.lblTradeList.Text = "导入分时交易明细";
			this.lblTradeList.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// btnImpTradeList
			// 
			this.btnImpTradeList.Location = new System.Drawing.Point(786, 405);
			this.btnImpTradeList.Margin = new System.Windows.Forms.Padding(0);
			this.btnImpTradeList.Name = "btnImpTradeList";
			this.btnImpTradeList.Size = new System.Drawing.Size(57, 24);
			this.btnImpTradeList.TabIndex = 10;
			this.btnImpTradeList.Text = "导入";
			this.btnImpTradeList.UseVisualStyleBackColor = true;
			this.btnImpTradeList.Click += new System.EventHandler(this.BtnImpTradeListClick);
			// 
			// btnSelTradeListDir
			// 
			this.btnSelTradeListDir.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.btnSelTradeListDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelTradeListDir.Location = new System.Drawing.Point(758, 405);
			this.btnSelTradeListDir.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelTradeListDir.Name = "btnSelTradeListDir";
			this.btnSelTradeListDir.Size = new System.Drawing.Size(24, 24);
			this.btnSelTradeListDir.TabIndex = 14;
			this.btnSelTradeListDir.Text = "...";
			this.btnSelTradeListDir.UseVisualStyleBackColor = true;
			this.btnSelTradeListDir.Click += new System.EventHandler(this.BtnSelTradeListDirClick);
			// 
			// btnSelKLineDir
			// 
			this.btnSelKLineDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.btnSelKLineDir.Location = new System.Drawing.Point(242, 22);
			this.btnSelKLineDir.Margin = new System.Windows.Forms.Padding(0);
			this.btnSelKLineDir.Name = "btnSelKLineDir";
			this.btnSelKLineDir.Size = new System.Drawing.Size(24, 24);
			this.btnSelKLineDir.TabIndex = 17;
			this.btnSelKLineDir.Text = "...";
			this.btnSelKLineDir.UseVisualStyleBackColor = true;
			this.btnSelKLineDir.Click += new System.EventHandler(this.BtnSelKLineDirClick);
			// 
			// lblKLineDir
			// 
			this.lblKLineDir.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.lblKLineDir.Location = new System.Drawing.Point(2, 22);
			this.lblKLineDir.Name = "lblKLineDir";
			this.lblKLineDir.Size = new System.Drawing.Size(36, 23);
			this.lblKLineDir.TabIndex = 20;
			this.lblKLineDir.Text = "目录:";
			this.lblKLineDir.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// tabRuleConf
			// 
			this.tabRuleConf.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.tabRuleConf.Location = new System.Drawing.Point(4, 23);
			this.tabRuleConf.Margin = new System.Windows.Forms.Padding(0);
			this.tabRuleConf.Name = "tabRuleConf";
			this.tabRuleConf.Size = new System.Drawing.Size(857, 444);
			this.tabRuleConf.TabIndex = 1;
			this.tabRuleConf.Text = "筛选规则设置";
			// 
			// dlgSelFile
			// 
			this.dlgSelFile.Multiselect = true;
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.lblProgressTitle);
			this.panel1.Controls.Add(this.lblProgressInfo);
			this.panel1.Controls.Add(this.progressBar);
			this.panel1.Controls.Add(this.lblProgressRate);
			this.panel1.Location = new System.Drawing.Point(4, 475);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(857, 44);
			this.panel1.TabIndex = 42;
			// 
			// lblProgressTitle
			// 
			this.lblProgressTitle.CausesValidation = false;
			this.lblProgressTitle.Location = new System.Drawing.Point(6, 2);
			this.lblProgressTitle.Name = "lblProgressTitle";
			this.lblProgressTitle.Size = new System.Drawing.Size(324, 18);
			this.lblProgressTitle.TabIndex = 40;
			this.lblProgressTitle.Text = "状态栏";
			// 
			// lblProgressInfo
			// 
			this.lblProgressInfo.CausesValidation = false;
			this.lblProgressInfo.Font = new System.Drawing.Font("微软雅黑", 9F);
			this.lblProgressInfo.Location = new System.Drawing.Point(395, 2);
			this.lblProgressInfo.Name = "lblProgressInfo";
			this.lblProgressInfo.Size = new System.Drawing.Size(458, 18);
			this.lblProgressInfo.TabIndex = 38;
			this.lblProgressInfo.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// progressBar
			// 
			this.progressBar.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
			this.progressBar.ForeColor = System.Drawing.Color.LimeGreen;
			this.progressBar.Location = new System.Drawing.Point(6, 20);
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(799, 18);
			this.progressBar.TabIndex = 37;
			// 
			// lblProgressRate
			// 
			this.lblProgressRate.CausesValidation = false;
			this.lblProgressRate.Font = new System.Drawing.Font("微软雅黑", 9F);
			this.lblProgressRate.Location = new System.Drawing.Point(811, 20);
			this.lblProgressRate.Name = "lblProgressRate";
			this.lblProgressRate.Size = new System.Drawing.Size(43, 18);
			this.lblProgressRate.TabIndex = 39;
			this.lblProgressRate.Text = "0%";
			this.lblProgressRate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
			// 
			// MainForm
			// 
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(866, 521);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.tabMainWindow);
			this.Font = new System.Drawing.Font("微软雅黑", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Analysis Tool";
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
