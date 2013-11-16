using System;
using StockEstimator.Contracts;
using StockEstimator.Service;
using System.Threading;
using WcfService;
using System.Collections.Generic;

namespace StockEstimator.Service.Main
{
    class MainClass
    {
        public static void Main (string[] args)
        {
			if(args.Length > 0)
			{
				var service = new EstimatorService();
				var list = service.GetMovingAverage5v30();
				foreach(var l in list)
				{
					Console.WriteLine (l.ToString());
				}
				
				list = new List<Symbol>();
				var estimator = new Estimator();
				foreach(var stock in estimator.Stocks)
				{
					if(service.MovingAverage5v30(stock.Symbol)){ list.Add(stock.Symbol);}
				}
				foreach(var l in list)
				{
					Console.WriteLine (l.ToString());
				}
				
				return;
			}
			
            try
	        {
				var serviceCreator = new ServiceCreator<EstimatorService, IEstimatorService>();	
                Console.WriteLine("Starting Service...");
				
			//	serviceCreator.Create();
				var thread = new Thread(serviceCreator.Create);
				thread.Start();
				Console.WriteLine("{0} Started.", typeof(EstimatorService).ToString());
				
				Thread.Sleep(10000);
				
				StockEstimator.Client.MainClass.Main(new string[0]);
				
				thread.Join();
	        }
			catch(AggregateException exception)
			{
				foreach(var e in exception.InnerExceptions)
				{
					Console.WriteLine("InnerException: {0}", e.Message);
				}
			}
	        catch (Exception exception)
	        {
	            Console.WriteLine("An exception occurred: {0}", exception.Message);
	        }
        }
    }
}
