using System;
using System.Linq;
using System.ServiceModel;
using StockEstimator;
using StockEstimator.Contracts;
using StockEstimator.InformationRepository;
using System.Collections.Generic;

namespace StockEstimator.Service
{
    [ServiceBehavior(Namespace = Constants.EstimatorNamespace)]
	public class EstimatorService : IEstimatorService
	{
		private GoogleStockProvider googleStockProvider{get; set;}
		private YahooStockProvider yahooStockProvider{get; set;}
		private Estimator estimator{get; set;}
		private DatabaseWriter writer{get; set;}
		public EstimatorService()
		{
			googleStockProvider = new GoogleStockProvider();
			yahooStockProvider = new YahooStockProvider();
			estimator = new Estimator();
			writer = new DatabaseWriter();
		}
		
		public bool Ping()
		{
			Console.WriteLine ("I have a ping!!!!");
			return true;
			
		}
		
		public bool UpdateStock(Symbol symbol)
		{
			try
			{
				Console.WriteLine("Updating {0} {1}", symbol, symbol.ToString());
				//It is possible here that we want to actually save the stock sources information to the database.	
	
				//Get previos data
				if(!writer.HasCachedBackHistory(symbol))
				{
					Console.WriteLine ("\tNo back history exists, so updating back history.");
					var histories = yahooStockProvider.GetStockHistory(symbol);
					writer.SaveStockHistory(symbol, histories);
				}
				Console.WriteLine ("\tChecking if trade exists for today.");
				//Do not save trades in the day trade table more than once a day
				if(!writer.TradeHistoryForDayExists(symbol, DateTime.Now.Date)) 
				{ 
					Console.WriteLine ("\tGetting stock info for today.");
					var dayHistory = googleStockProvider.GetStockInformation(symbol);
					if(dayHistory != null)
					{
						Console.WriteLine ("\tSave stock information.");
						writer.SaveDayHistory(dayHistory);
					}
				}
			}
			catch(Exception exception)
			{
				throw new FaultException<Exception>(exception);
			}
			return true;
		}
		
		public void Update()
		{
			
			try
			{
				Console.WriteLine ("I have an update!!!!");
				//It is possible here that we want to actually save the stock sources information to the database.
				var estimator = new Estimator();
				var writer = new DatabaseWriter();
				
				foreach(var stock in estimator.Stocks)
				{				
					Console.WriteLine ("Stock: {0}", stock.Symbol.ToString());
					//Get previos data
					if(!writer.HasCachedBackHistory(stock.Symbol))
					{
						Console.WriteLine ("No back history exists, so updating back history.");
						var histories = new YahooStockProvider().GetStockHistory(stock.Symbol);
						writer.SaveStockHistory(stock.Symbol, histories);
					}
					Console.WriteLine ("Checking if trade exists for today.");
					//Do not save trades in the day trade table more than once a day
					if(writer.TradeHistoryForDayExists(stock.Symbol, DateTime.Now.Date)) { continue; }
					
					Console.WriteLine ("Get stock info for today.");
					var dayHistory = googleStockProvider.GetStockInformation(stock.Symbol);
					if(dayHistory != null)
					{
						Console.WriteLine ("Save stock information.");
						writer.SaveDayHistory(dayHistory);
					}
				}
			}
			catch(Exception exception)
			{
				throw new FaultException<Exception>(exception);
			}
		}
		
		public bool IsServiceCacheCurrent()
		{
			Console.WriteLine ("I have a current!!!!");
			var writer = new DatabaseWriter();
			return writer.DoesTradeInfoExistForDate(DateTime.Now.Date);
		}
		
		public List<Symbol> GetStrategy1()
		{
			var writer = new DatabaseWriter();
			return writer.Strategy1();
		}
		
		public StockPrediction GetStockPredictionRandom(Symbol symbol)
		{
			return new StockPrediction{ Delta = 5 - 10 * new Random().NextDouble()};
		}
		
		public IList<Symbol> GetMovingAverage5v30()
		{
			var list = new List<Symbol>();
			foreach(var stock in estimator.Stocks)
			{
				if(MovingAverage5v30(stock.Symbol)){ list.Add(stock.Symbol);}
			}
			return list;
		}
		
		public bool MovingAverage5v30(Symbol symbol)
		{
			try
			{
				//If the average is moving up you buy
				//mowing averages 5 day to 30 day to 200 day
				
				//when the most recent setup passes over longer
				//so lets do the 5 versus 30
				var historyInformation = writer.Get200DayStockPrices(symbol).OrderByDescending(s => s.TradeDate);
				if(historyInformation.Count() < 30) 
				{
					Console.WriteLine("Not enough data points to check stock {0}, size: {1}", symbol, historyInformation.Count());
					return false;
				}
				var avg5Day = historyInformation.Take(5).Average(h => h.Price);
				var avg30Day = historyInformation.Take(30).Average(h => h.Price);
				
				return avg5Day > avg30Day;
			}
			catch(Exception exception)
			{
				Console.WriteLine("Exception here: {0}{1}{2}", exception.Message, Environment.NewLine, exception.StackTrace);
				return false;
			}
		}
		
		public bool MovingAverage5v30Derivative(Symbol symbol)
		{
			return false;
		}
		
		public bool IsItForecastedToGoUp(Symbol symbol)
		{
			return MovingAverage5v30(symbol);
		}
		public bool DoOtherPeopleAleadyKnowABoutThis(Symbol symbol){ return false; }
		public bool WillThisTradeBeProfitable(Trade trade)
		{
			//Compare current holding for stock versus how what the trade is
			//if we want 500 shares and have 200, then only buy 300, or possibly 400 based on the
			//new information we have obtained.
			return false; 
		}
		public bool ProfitRiskCalculation(Symbol symbol){ return false; }
	}
}

