using System;
using System.Collections.Generic;
using StockEstimator.Contracts;

namespace StockEstimator.Service
{
	public interface IStockProvider
	{
		IDayHistory GetStockInformation(Symbol symbol);
		IList<IStockHistory> GetStockHistory(Symbol symbol);
	}
}

