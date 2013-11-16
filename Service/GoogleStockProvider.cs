using System;
using System.Xml.Linq;
using StockEstimator;
using System.Collections.Generic;
using StockEstimator.Contracts;

namespace StockEstimator.Service
{
	public class GoogleStockProvider : IStockProvider
	{
		public GoogleStockProvider()
		{
			Console.WriteLine ("Google provider created.");
		}
		
		public IDayHistory GetStockInformation(Symbol symbol)
		{
			var url = String.Empty;
			XDocument doc = null;
			try
			{
				url = String.Format("http://www.google.com/ig/api?stock={0}", symbol);
	            doc = XDocument.Load(url);
			}
			catch(Exception exception)
			{
				Console.WriteLine ("Could not use xdocumunt to load the url: {0}", exception.Message);
				return null;
			}
			try
			{
				return new DayHistory(symbol){
					Company = GetData(doc, "company"),
		            Exchange = GetData(doc, "exchange"),
					Open = Convert.ToDecimal(GetData(doc, "open")),
					Close = Convert.ToDecimal(GetData(doc, "y_close")),
					Volume = Convert.ToDouble(GetData(doc, "volume")),
					AvgVolume = Convert.ToDecimal(GetData(doc, "avg_volume")),
					MarketCap = Convert.ToDecimal(GetData(doc, "market_cap")),
		            Last = Convert.ToDecimal(GetData(doc, "last")),
		            High = Convert.ToDecimal(GetData(doc, "high")),
		            Low = Convert.ToDecimal(GetData(doc, "low")),
					TradeTime = GetDate(GetData(doc, "trade_date_utc"), GetData(doc, "trade_time_utc")).ToLocalTime()
				};
			}
			catch(Exception exception)
			{
				Console.WriteLine ("Could not parse xml document: {0}", exception.Message);
				return null;
			}
			
		}
		
        private string GetData(XDocument doc, string name)
        {
            return doc.Root.Element("finance").Element(name).Attribute("data").Value;
        }
		
		private DateTime GetDate(String dateStr, String timeStr)
		{
			var format = String.Format("{0}-{1}-{2}T{3}:{4}:{5}",
					dateStr.Substring(0,4), dateStr.Substring(4,2), dateStr.Substring(6,2), 
                    timeStr.Substring(0,2), timeStr.Substring(2,2), timeStr.Substring(4,2));
			return DateTime.Parse(format);
		}	
		
		public IList<IStockHistory> GetStockHistory(Symbol symbol)
		{
			throw new NotImplementedException();
		}
	}
}


