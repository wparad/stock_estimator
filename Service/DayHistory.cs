using System;
using System.Collections.Generic;

namespace StockEstimator
{
	public class DayHistory : IDayHistory
	{
		public Symbol Symbol{ get; private set; }
	
		public String Company { get; set; }
	    public String Exchange { get; set; }
		public decimal Last { get; set; }
	    public decimal High { get; set; }
	    public decimal Low { get; set; }
		
		public decimal Open { get; set; }
		public decimal Close { get; set; }
		public double Volume { get; set; }
		public decimal AvgVolume { get; set; }
		public decimal MarketCap { get; set; }
	    public DateTime TradeTime { get; set; }
		
		public DayHistory (Symbol symbol)
		{
			Symbol = symbol;
		}
	}
}

