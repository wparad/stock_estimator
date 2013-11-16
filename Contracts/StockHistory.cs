using System;

namespace StockEstimator.Contracts
{
	public class StockHistory : IStockHistory
	{
		public decimal Last { get; set;}
	    public decimal High { get; set; }
	    public decimal Low { get; set; }
		
		public decimal Open { get; set; }
		public decimal Close { get; set; }
		public double Volume { get; set; }
		public decimal AvgVolume { get; set; }
		public DateTime TradeTime { get; set; }
		public StockHistory ()
		{
		}
	}
}

