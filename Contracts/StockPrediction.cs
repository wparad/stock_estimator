using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace StockEstimator.Contracts
{
	[DataContract]
	public class StockPrediction
	{
		[DataMember]
		public double Confidence { get; set; }
		
		[DataMember]
		public double Delta { get; set; }
		
		[DataMember]
		public DateTime InvalidityTime { get; set; }
		
		public StockPrediction ()
		{
			Confidence = 0;
			Delta = 0;
			InvalidityTime = DateTime.Now.AddDays(5);
		}
	}
}

