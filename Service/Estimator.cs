using System;
using System.Collections.Generic;
using StockEstimator.Contracts;

namespace StockEstimator.Service
{
	public class Estimator : IEstimator
	{
		public ICollection<IStock> Stocks {get; private set;}
		public Estimator ()
		{
			Stocks = new List<IStock>();
			foreach(Symbol symbol in Enum.GetValues(typeof(Symbol))) 
			{
				Stocks.Add(new Stock(){Symbol = symbol});
			}
		}
		
		public IList<IDayHistory> CalculateInterestingStocks()
		{
			var stocks = new List<IDayHistory>();
			return stocks;
		}
	}
}

