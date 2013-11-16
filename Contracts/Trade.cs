using System;
using System.Runtime.Serialization;

namespace StockEstimator.Contracts
{
	public enum TradeType
	{
		BUY,
		SELL
	}
	
	
	[DataContract]
	public class Trade
	{
		[DataMember]
		public Symbol symbol { get; set; }
		
		[DataMember]
		public int shares { get; set; }
		
		[DataMember]
		public TradeType TradeTrade { get; set; }
		
		public Trade ()
		{
		}
	}
}

