using System;
using StockEstimator;

namespace StockEstimator.Service
{
	public interface IStock
	{
		Symbol Symbol {get;}
		bool HasCachedBackHistory {get;}
	}
}

