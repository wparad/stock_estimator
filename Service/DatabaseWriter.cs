using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using MySql.Data;
using MySql.Data.MySqlClient;
using StockEstimator.Contracts;

namespace StockEstimator.InformationRepository
{
	public class DatabaseWriter : DatabaseAccess
	{
		public DatabaseWriter ()
		{
		}
		
		public void SaveDayHistory(IDayHistory stock)
		{
			//Even if we checked before, the best we could have done is the current date, 
			//now that we actually have a stock we can check the info that will actually be saved
			if(TradeHistoryForDayExists(stock.Symbol, stock.TradeTime)) { return; }
			
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Symbol", stock.Symbol.ToString()));
			parameters.Add(new MySqlParameter("@High", stock.High));
			parameters.Add(new MySqlParameter("@Low", stock.Low));
			parameters.Add(new MySqlParameter("@Last", stock.Last));
			parameters.Add(new MySqlParameter("@Open", stock.Open));
			parameters.Add(new MySqlParameter("@Close", stock.Close));
			parameters.Add(new MySqlParameter("@Volume", stock.Volume));
			parameters.Add(new MySqlParameter("@AvgVolume", stock.AvgVolume));
			parameters.Add(new MySqlParameter("@TradeTime", stock.TradeTime.Date));
			
			ExecuteNonQuery(@"INSERT INTO DayHistory(Symbol, Last, High, Low, Open, Close, Volume, AvgVolume, TradeTime)
								VALUES (@Symbol, @Last, @High, @Low, @Open, @Close, @Volume, @AvgVolume, @TradeTime);", parameters);
		}
		
		public void SaveStockHistory(Symbol symbol, IList<IStockHistory> stockHistories)
		{
			var currentHistories = GetDayHistories(symbol);
			foreach(var history in stockHistories.Where(h => !currentHistories.ContainsKey(h.TradeTime)))
			{ 
				var parameters = new List<MySqlParameter>();
				parameters.Add(new MySqlParameter("@Symbol", symbol.ToString()));
				parameters.Add(new MySqlParameter("@High", history.High));
				parameters.Add(new MySqlParameter("@Low", history.Low));
				parameters.Add(new MySqlParameter("@Last", history.Last));
				parameters.Add(new MySqlParameter("@Open", history.Open));
				parameters.Add(new MySqlParameter("@Close", history.Close));
				parameters.Add(new MySqlParameter("@Volume", history.Volume));
				parameters.Add(new MySqlParameter("@AvgVolume", history.AvgVolume));
				parameters.Add(new MySqlParameter("@TradeTime", history.TradeTime.Date));
				
				ExecuteNonQuery(@"INSERT INTO DayHistory(Symbol, Last, High, Low, Open, Close, Volume, AvgVolume, TradeTime)
									VALUES (@Symbol, @Last, @High, @Low, @Open, @Close, @Volume, @AvgVolume, @TradeTime);", parameters);
			}
		}
		
		public IDictionary<DateTime, IDayHistory> GetDayHistories(Symbol symbol)
		{
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Symbol", symbol.ToString()));
			
			var dayHistories = new Dictionary<DateTime, IDayHistory>();
			ExecuteQuery("SELECT * FROM DayHistory s WHERE s.Symbol = @Symbol;", parameters, command => 
            {
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						var tradeTime = reader.GetDateTime("TradeTime");
						var avgVolume = reader.GetDecimal("AvgVolume"); 
						var close = reader.GetDecimal("Close");
						var high = reader.GetDecimal("High");
						var last = reader.GetDecimal("Last");
						var low = reader.GetDecimal("Low");
						var open = reader.GetDecimal("Open");
						var volume = reader.GetDouble("Volume");
						dayHistories.Add(tradeTime, new DayHistory(symbol)
		                { 
							AvgVolume = avgVolume,
							Close = close,
							High = high,
							Last = last,
							Low = low,
							Open = open,
							TradeTime = tradeTime,
							Volume = volume
						});
					}
				}
			});
			return dayHistories;
		}	
			
		public bool TradeHistoryForDayExists(Symbol symbol, DateTime tradeTime)
		{
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Symbol", symbol.ToString()));
			parameters.Add(new MySqlParameter("@TradeTime", tradeTime.Date));
			
			var result = false;
			ExecuteQuery("SELECT * FROM DayHistory s WHERE s.Symbol = @Symbol AND s.TradeTime = @TradeTime;", parameters, command => 
            {
				using(var reader = command.ExecuteReader())
				{
					result = reader.HasRows;
				}
			});
			return result;
		}
		
		public bool HasCachedBackHistory(Symbol symbol)
		{
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Symbol", symbol.ToString()));
			parameters.Add(new MySqlParameter("@TradeTime", DateTime.Now.Date));
			
			var result = false;
			ExecuteQuery("SELECT Count(*) AS Count FROM DayHistory s WHERE s.Symbol = @Symbol AND s.TradeTime < @TradeTime;", parameters, command => 
            {
				using(var reader = command.ExecuteReader())
				{
					result = reader.Read() && reader.HasRows && reader.GetInt32("Count") > 5;
				}
			});
			return result;
		}
		
		public bool DoesTradeInfoExistForDate(DateTime dateTime)
		{
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@TradeTime", dateTime));
			Console.WriteLine ("DateTime inside DoesTradeInfoExistForDate: {0}", dateTime);
			
			var result = false;
			ExecuteQuery("SELECT Count(*) as Count FROM DayHistory s WHERE s.TradeTime = @TradeTime;", parameters, command => 
            {
				var count = Convert.ToInt64(command.ExecuteScalar());
				
				result = count > 0;
				Console.WriteLine ("value inside DoesTradeInfoExistForDate: {0}", result);
			});
			return result;
		}
		
		public void LogException(Exception exception)
		{
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Message", String.Format("{0}:{1}", exception.Message, exception.StackTrace)));
			ExecuteNonQuery("INSERT INTO LoggedServerMessages(Message) VALUES(@Message);", parameters);
		}
		
		public List<Symbol> Strategy1()
		{
			var symbols = new List<Symbol>();
			var parameters = new List<MySqlParameter>();
			ExecuteQuery(
				@"select DISTINCT Symbol from DayHistory 
				where Volume > 400000 AND TradeTime > '2010-01-01' order by TradeTime desc, Volume desc limit 13;", parameters, command => 
			{
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						symbols.Add((Symbol)Enum.Parse(typeof(Symbol), reader.GetString("Symbol")));
					}
				}
			});
			return symbols;
		}
		
		public IList<Contracts.DayHistory> Get200DayStockPrices(Symbol symbol)
		{
			var list = new List<Contracts.DayHistory>();
			var parameters = new List<MySqlParameter>();
			parameters.Add(new MySqlParameter("@Today", DateTime.Now.AddDays(-30.0).Date));
			parameters.Add(new MySqlParameter("@Symbol", symbol));
			ExecuteQuery(
				@"select Last, TradeTime from DayHistory 
				where TradeTime > @Today AND Symbol = @Symbol;", parameters, command => 
			{
				using(var reader = command.ExecuteReader())
				{
					while(reader.Read())
					{
						list.Add(new Contracts.DayHistory{Price = reader.GetDouble("Last"), 
							TradeDate = reader.GetDateTime("TradeTime") });
					}
				}
			});
			return list;
		}
	}
}

