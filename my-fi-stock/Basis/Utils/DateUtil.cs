using System;
using System.Text.RegularExpressions;

namespace Pandora.Basis.Utils
{
	public static class DateUtil
	{
		private static Regex datePattern = new Regex("^[0-9]{4,4}-[0-9]{2,2}-[0-9]{2,2}$"
		                      , RegexOptions.Singleline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
		
		/// <summary>
		/// 获取<paramref name="date"/>所在季度第一天的日期
		/// </summary>
		/// <param name="date"></param>
		/// <returns></returns>
		public static DateTime GetQuarterFirstDay(DateTime date){
			int quarter = (date.Month + 2) / 3;
			return new DateTime(date.Year, (quarter-1) * 3 + 1, 1);
		}
		
		public static int GetQuarter(DateTime date){
			return (date.Month + 2) / 3;
		}
		
		public static bool IsStdDateString(string str){
			if(string.IsNullOrEmpty(str) || str.Trim().Length<=0) return false;
			return datePattern.IsMatch(str);
		}
	}
}
