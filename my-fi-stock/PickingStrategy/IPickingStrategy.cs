using System;
using System.Collections.Generic;
using Pandora.Invest.Entity;
using Pandora.Basis.DB;

namespace Pandora.Invest.PickingStrategy
{
	/// <summary>
	/// 选股策略
	/// </summary>
	public interface IPickingStrategy
	{
		/// <summary>
		/// 执行策略选股。
		/// 1.运用策略从量价特征中匹配出符合条件的买点；
		/// 2.对买点后续价格趋势进行判断，确定买点是否成功；
		/// </summary>
		/// <param name="conf">选股模式。全量搜索或者最近交易日搜索</param>
		/// <param name="sto">股票</param>
		/// <param name="kdatas">全部日线数据，按日期升序排序</param>
		/// <param name="masList">全部价格短期趋势，按日期升序排序</param>
		/// <param name="malList">全部价格长期趋势，按日期升序排序</param>
		/// <param name="vmalList">全部成交量长期趋势，按日期升序排序</param>
		/// <returns></returns>
		IList<PickingResult> DoPicking(StrategyConfig conf, Stock sto, IList<KJapaneseData> kdatas
			, IList<KTrendMAShort> masList, IList<KTrendMALong> malList, IList<KTrendVMALong> vmalList);
	}
}
