using System;
using System.Collections.Generic;
using Pandora.Invest.Entity;
using Pandora.Basis.DB;

namespace Pandora.Invest.Rule
{
	public interface IStockFilter
	{
		IList<Stock> DoFilter(Database db, IList<Stock> stocks);
	}
}