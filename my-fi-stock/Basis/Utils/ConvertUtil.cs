using System;
using System.Collections.Generic;
using System.Text;

namespace Pandora.Basis.Utils
{
	/// <summary>
	/// utilities that help to convert data between different types
	/// </summary>
	public static class ConvertUtil
	{
		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="string"/> value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static string ToString(object value, string @default)
		{
			if (value == null) return @default;
			return value.ToString();
		}

		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="int"/> value
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static int ToInt(object obj, int @default)
		{
			if (obj == null) return @default;
			int val = 0;
			if (!Int32.TryParse(obj.ToString(), out val))
				val = @default;
			return val;
		}
		
		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="long"/> value
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static long ToLong(object obj, long @default)
		{
			if (obj == null) return @default;
			long val = 0;
			if (!Int64.TryParse(obj.ToString(), out val))
				val = @default;
			return val;
		}
		
		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="decimal"/> value
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static decimal ToDecimal(object obj, decimal @default)
		{
			if (obj == null) return @default;
			decimal val = 0;
			if (!decimal.TryParse(obj.ToString(), out val))
				val = @default;
			return val;
		}
		
		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="decimal"/> value
		/// </summary>
		/// <param name="obj"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static bool ToBoolean(object obj, bool @default)
		{
			if (obj == null) return @default;
			string sVal = obj.ToString().ToLower().Trim();
			if(string.IsNullOrEmpty(sVal)) return @default;
			if(sVal=="false" || sVal=="0" || sVal=="f") return false;
			return true;
		}

		/// <summary>
		/// Convert <see cref="object"/> type value to <see cref="DateTime"/> value
		/// </summary>
		/// <param name="value"></param>
		/// <param name="default">The default value if the object is null</param>
		/// <returns></returns>
		public static DateTime ToDateTime(object value, DateTime @default)
		{
			if (value == null) return @default;
			if (value.GetType() == typeof(DateTime)) return (DateTime)value;
			DateTime datetime = @default;
			if (!DateTime.TryParse(value.ToString(), out datetime))
				datetime = @default;
			return datetime;
		}
	}
}