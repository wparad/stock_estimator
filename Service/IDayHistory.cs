using System;
using System.Collections.Generic;

namespace StockEstimator
{
	public interface IDayHistory
	{
		Symbol Symbol{ get;}
		
		String Company { get; }
	    String Exchange { get; }
		decimal Last { get; }
	    decimal High { get; }
	    decimal Low { get; }
		
		decimal Open { get; }
		decimal Close { get; }
		double Volume { get; }
		decimal AvgVolume { get; }
		decimal MarketCap { get; }
	    DateTime TradeTime { get; }
	}
}

