using System;
using System.Collections.Generic;
using StockEstimator.Contracts;

namespace StockEstimator.Service
{
	public interface IEstimator
	{
		ICollection<IStock> Stocks {get;}
		IList<IDayHistory> CalculateInterestingStocks();
	}
}

