using System;
using WcfService;
using System.ServiceModel;
using System.Collections.Generic;

namespace StockEstimator.Contracts
{
	//TODO can endpoint address be moved into the ServiceContract attribute? 
	//No because that is an implementation detail not contractlevel
	//But since you always want clients to list on that port, adding this will tell them where to go
	[ZService(EndpointAddress = Constants.EndpointAddress)]
    [ServiceContract(Namespace = Constants.EstimatorNamespace)]
    public interface IEstimatorService
    {
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool Ping();
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
        bool UpdateStock(Symbol symbol);
		
        [OperationContract]
		[FaultContract(typeof(Exception))]
        void Update();
	
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool IsServiceCacheCurrent();
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		List<Symbol> GetStrategy1();
		
		/// <summary>
		/// Get the predicted stock going up or down
		/// </summary>
		/// <returns>
		/// The stock prediction.
		/// </returns>
		/// <param name='symbol'>
		/// Symbol.
		/// </param>
		[OperationContract]
		[FaultContract(typeof(Exception))]
		StockPrediction GetStockPredictionRandom(Symbol symbol);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		IList<Symbol> GetMovingAverage5v30();
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool MovingAverage5v30(Symbol symbol);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool MovingAverage5v30Derivative(Symbol symbol);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool IsItForecastedToGoUp(Symbol symbol);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool DoOtherPeopleAleadyKnowABoutThis(Symbol symbol);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool WillThisTradeBeProfitable(Trade trade);
		
		[OperationContract]
		[FaultContract(typeof(Exception))]
		bool ProfitRiskCalculation(Symbol symbol);
    }
}