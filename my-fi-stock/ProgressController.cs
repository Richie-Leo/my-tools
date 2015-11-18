using System;
using System.Windows.Forms;

using Pandora.Invest.DataCapture;

namespace Pandora.Invest
{
	/// <summary>
	/// 进度条控制器
	/// </summary>
	public class ProgressController
	{
		private Timer _timer;
		private ProgressBar _progressBar;
		private Label _lblTitle;
		private Label _lblInfo;
		private Label _lblRate;
		
		private ProgressStatus _status;
		private bool _initialized = false;
		
		public ProgressController(Timer timer, ProgressBar bar, Label lblTitle, Label lblInfo, Label lblRate){
			this._timer = timer;
			this._timer.Tick += new EventHandler(this.OnTimer_UpdateProgressBar);
			this._timer.Interval = 800;
			this._progressBar = bar;
			this._lblTitle = lblTitle;
			this._lblInfo = lblInfo; 
			this._lblRate = lblRate;
		}
		
		public void Start(string title, ProgressStatus status){
			this._lblTitle.Text = title;
			this._status = status;
			this._initialized = false;
			this._progressBar.Minimum = 0;
			this._progressBar.Value = 0;
			this._timer.Enabled = true;
		}
		
		private void OnTimer_UpdateProgressBar(object sender, EventArgs e){
			if(!this._initialized){
				if(!(this._status.IsRunning || this._status.HasFinished)) return;
				this._progressBar.Maximum = _status.TotalNum;
				this._initialized = true;
			}
			this._progressBar.Value = _status.FinishedNum;
			if(_status.HasFinished){
				this._timer.Enabled = false;
				ProgressStatus status = this._status;
				this._status = null;
				this._lblRate.Text = "100%";
				this._lblInfo.Text = "总数：" + status.FinishedNum + "，剩余：0，用时：" + status.ElapsedSeconds.ToString("F1") + "秒";
			}else{
				this._lblRate.Text = _status.CurrentRate.ToString("F1") + "%";
				this._lblInfo.Text = "总数：" + _status.TotalNum
					+ "，剩余：" + _status.RemainingNum + "，用时："
					+ _status.ElapsedSeconds.ToString("F1") + "秒";
			}
		}
	}
}
