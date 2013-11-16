using System;
using System.Data;
using System.Linq;
using MySql.Data.MySqlClient;
using System.Collections.Generic;

namespace StockEstimator.InformationRepository
{
	public abstract class DatabaseAccess 
	{
		protected static MySqlConnectionStringBuilder builder = new MySqlConnectionStringBuilder()
		{
			Server = "192.168.2.60",
			Port = 65501,
			Database = "Stocks",
			UserID = "warren",
			Password = "PASSWORD"
		};
	
		static DatabaseAccess()
		{
			try
			{
				var cs = new MySqlConnectionStringBuilder(builder.ConnectionString){Database = String.Empty}.ConnectionString;
				using (var connection = new MySqlConnection(cs))
				using (var command = connection.CreateCommand())
				{
				    connection.Open();
					
					command.CommandText = "CREATE DATABASE IF NOT EXISTS `Stocks`;";
				    command.ExecuteNonQuery();
					
					command.CommandText = @"CREATE TABLE IF NOT EXISTS Stocks.DayHistory(
												DayHistoryNbr BIGINT PRIMARY KEY NOT NULL AUTO_INCREMENT,
												Symbol VARCHAR(10) NOT NULL,
												Last DECIMAL(20,10) NOT NULL,
												High DECIMAL(20,10),
												Low DECIMAL(20,10),
												Open DECIMAL(20,10),
												Close DECIMAL(20,10),
												Volume DECIMAL(25,2),
												AvgVolume DECIMAL(25,5) NOT NULL DEFAULT 0,
												TradeTime TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
												INDEX IX_StockHistory_Symbol_TradeTime (Symbol, TradeTime),
												CONSTRAINT UNIQUE INDEX UQ_DayHistory_Symbol_Day (Symbol, TradeTime));";
					command.ExecuteNonQuery();
					
					command.CommandText = @"CREATE TABLE IF NOT EXISTS Stocks.LoggedServerMessages(
									        	LoggedServerMessagesNbr BIGINT PRIMARY KEY NOT NULL AUTO_INCREMENT,
									        	TimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
									        	Message varchar(10000),
									        	INDEX IX_LoggedServerMessages_TimeStamp (TimeStamp));";
					command.ExecuteNonQuery();
					
					command.CommandText = @"CREATE TABLE IF NOT EXISTS Stocks.ServerCommand(
										        ServerCommand BIGINT PRIMARY KEY NOT NULL AUTO_INCREMENT,
										        Message varchar(2000),
										        TimeStamp TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
										        INDEX IX_ServerCommand_TimeStamp (TimeStamp));";
					command.ExecuteNonQuery();
					
					connection.Close();
				}	
			}
			catch(MySqlException exception)
			{
				Console.WriteLine(exception.Message);
			}
		}
		
		public bool Success;
		
		public void ExecuteNonQuery(String commandText, IList<MySqlParameter> paramaters)
		{
			ExecuteQuery(commandText, paramaters, command => command.ExecuteNonQuery());
		}
		
		public void ExecuteQuery(String commandText, IList<MySqlParameter> paramaters, Action<MySqlCommand> action)
		{
			try
			{
				using(var connection = new MySqlConnection(builder.ConnectionString))
				{
					connection.Open();
					try
					{
						using(var command = connection.CreateCommand())
						{
							command.CommandText = commandText;
							var parameterText = String.Empty;
							if(paramaters.Count != 0)
							{
								parameterText = paramaters.Select(p => String.Format("{0}={1}", p.ParameterName, p.Value))
									.Aggregate( (seq, next) => String.Format("{0}, {1}", seq, next));
							}
							foreach(var parameter in paramaters)
							{
								command.Parameters.AddWithValue(parameter.ParameterName, parameter.Value);
							}
							
							try
							{
								action(command);
							}
							finally
							{	
								using (var commandLogger = connection.CreateCommand())
								{
									commandLogger.CommandText = "INSERT INTO ServerCommand(Message) VALUES(@Message);";
									commandLogger.Parameters.AddWithValue("@Message", String.Format("{0}: {1}", command.CommandText, parameterText));
							    	commandLogger.ExecuteNonQuery();
								}
							}
						}
					}
					catch(Exception exception)
					{
						Console.WriteLine ("{0}\n{1}",exception.Message, exception.GetType().ToString());
						Console.WriteLine (exception.StackTrace);
					}
					finally
					{
						connection.Close();
					}
				}
			}
			catch(Exception exception)
			{
				Console.WriteLine ("{0}\n{1}",exception.Message, exception.GetType().ToString());
				Console.WriteLine (exception.StackTrace);
			}
		}
	}
}


