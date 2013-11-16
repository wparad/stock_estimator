using System;
using System.Runtime.Serialization;

namespace StockEstimator.Contracts
{
	[DataContract]
	public class DayHistory
	{
		[DataMember]
		public DateTime TradeDate {get; set;}
		
		[DataMember]
		public double Price { get; set; }
		public DayHistory ()
		{
		}
	}
}

