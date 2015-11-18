using System;
using System.Text;
using System.Collections.Generic;
using Pandora.Basis.Utils;

namespace Pandora.Invest.PickingStrategy
{
	/// <summary>
	/// 策略配置
	/// </summary>
	public class StrategyConfig
	{
		private IDictionary<string, string> _parameters = new Dictionary<string, string>();
		
		public string StrategyClass { get; set; }
		
		public StrategyConfig(string strategyClass, string confString){
			this.StrategyClass = strategyClass;
			
			this._parameters.Clear();
			if(string.IsNullOrEmpty(confString)) return;
			string[] paramPairs = confString.Split(';');
			for(int i=0;i <paramPairs.Length; i++){
				string pair = paramPairs[i].Trim();
				if(string.IsNullOrEmpty(pair)) continue;
				string[] nameValuePair = pair.Split('=');
				if(nameValuePair.Length!=2)
					throw new Exception("Conf string error for " + this.StrategyClass + ": " + pair);
				this._parameters.Add(nameValuePair[0].Trim(), nameValuePair[1].Trim());
			}
		}
		public string ConfigString{
			get{
				StringBuilder sb = new StringBuilder();
				int index = 0;
				foreach(KeyValuePair<string, string> kv in this._parameters){
					sb.Append(kv.Key).Append('=').Append(kv.Value);
					if(index<this._parameters.Count) sb.Append("; ");
					index++;
				}
				return sb.ToString();
			}
		}
		
		public string GetString(string name){
			if(this._parameters.ContainsKey(name)) return this._parameters[name];
			return string.Empty;
		}
		public decimal GetDecimal(string name){
			if(this._parameters.ContainsKey(name)) 
				return ConvertUtil.ToDecimal(this._parameters[name], 0);
			return 0;
		}
		public int GetInt(string name){
			if(this._parameters.ContainsKey(name)) 
				return ConvertUtil.ToInt(this._parameters[name], 0);
			return 0;
		}
		public long GetLong(string name){
			if(this._parameters.ContainsKey(name)) 
				return ConvertUtil.ToLong(this._parameters[name], 0);
			return 0;
		}
		public bool GetBool(string name){
			if(this._parameters.ContainsKey(name)) 
				return ConvertUtil.ToBoolean(this._parameters[name], false);
			return false;
		}
		public DateTime GetDateTime(string name){
			if(this._parameters.ContainsKey(name)) 
				return ConvertUtil.ToDateTime(this._parameters[name], DateTime.MinValue);
			return DateTime.MinValue;
		}
	}
}
