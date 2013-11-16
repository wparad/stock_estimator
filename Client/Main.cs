using System;
using System.Linq;
using System.ServiceModel;
using StockEstimator.Contracts;
using System.Threading;
using WcfService;
using System.Collections.Generic;

namespace StockEstimator.Client
{
	public class MainClass
	{
		public static void Main (string[] args)
		{
			var estimatorService = ServiceClient<IEstimatorService>.Service;
			
			var result = estimatorService.Ping();
			Console.WriteLine ("Ping: {0}", result);
			result = estimatorService.IsServiceCacheCurrent();
			Console.WriteLine ("Current: {0}", result);
			
			var list = new List<Symbol>();
			foreach(Symbol symbol in Enum.GetValues(typeof(Symbol)))
			{
				Console.WriteLine("Updating: {0}", symbol);
				try
				{
					var updated = estimatorService.UpdateStock(symbol);
					Console.WriteLine("Is updated: {0}", updated);
					if(estimatorService.MovingAverage5v30(symbol)){ list.Add(symbol); }
				}
				catch(FaultException exception)
				{
					Console.WriteLine("\tServer error updating {0}", exception.Message);
					if(exception.InnerException != null)
					{
						Console.WriteLine ("Message: {0}{1}{2}", 
					                  exception.InnerException.Message, Environment.NewLine, exception.StackTrace);
					}
				}
			}
		}
	}
}
