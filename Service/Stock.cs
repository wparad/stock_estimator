using System;
using System.Runtime.Serialization;

namespace StockEstimator.Service
{
	public class Stock : IStock
	{
		public Symbol Symbol { get; set; }
		public bool HasCachedBackHistory { get; set; }
	}
}			