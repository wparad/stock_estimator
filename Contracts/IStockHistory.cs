using System;

namespace StockEstimator.Contracts
{
	public interface IStockHistory
	{
		decimal Last { get; }
	    decimal High { get; }
	    decimal Low { get; }
		
		decimal Open { get; }
		decimal Close { get; }
		double Volume { get; }
		decimal AvgVolume { get; }
		DateTime TradeTime { get; }
	}
}

