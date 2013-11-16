using System;
using System.Linq;
using System.Net;
using System.IO;
using System.Security.Policy;
using System.Xml.Linq;
using StockEstimator;
using System.Collections.Generic;
using StockEstimator.Contracts;

namespace StockEstimator.Service
{
	public class YahooStockProvider : IStockProvider
	{
		public YahooStockProvider()
		{
		}
		
		public IDayHistory GetStockInformation(Symbol symbol)
		{
			//var multiStock = "http://finance.yahoo.com/d/quotes.csv?s=RHT+MSFT&f=sb2b3jk";
			var url = String.Format("http://finance.yahoo.com/d/quotes.csv?s={0}&f=sb2b3jk", symbol.ToString());
			
			var objStream  = WebRequest.Create(url).GetResponse().GetResponseStream();
			
			using(var objReader = new StreamReader(objStream))
			{
				String line;
				while ((line = objReader.ReadLine()) != null)
				{
					Console.WriteLine(line);
				}
			}
			return new DayHistory(symbol);
		}
		
		public IList<IStockHistory> GetStockHistory(Symbol symbol)
		{
			var url = String.Format("http://ichart.finance.yahoo.com/table.csv?s={0}&d=7&e=5&f=2013&g=d&a=3&b=12&c=1970&ignore=.csv", symbol.ToString());
			
			var objStream  = WebRequest.Create(url).GetResponse().GetResponseStream();
			
			var stockHistories = new List<IStockHistory>();
			using(var objReader = new StreamReader(objStream))
			{
				String line;
				while ((line = objReader.ReadLine()) != null)
				{
					var columns = line.Split(',');
					if(columns[0].Contains("Date")) { continue;}
					
					stockHistories.Add(new StockHistory
    	            {
						TradeTime = DateTime.Parse(columns[0]),
						Open = decimal.Parse(columns[1]),
						High = decimal.Parse(columns[2]),
						Low = decimal.Parse(columns[3]),
						Close = decimal.Parse(columns[4]),
						Volume = double.Parse(columns[5]),
						Last  = decimal.Parse(columns[4]),
						AvgVolume = 0
					});
				}
			}
			return stockHistories;
		}
	}
}

