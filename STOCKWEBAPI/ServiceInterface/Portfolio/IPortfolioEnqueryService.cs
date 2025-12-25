using System;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.DataEntities.RootStackx;

namespace STOCKWEBAPI.ServiceInterface.Portfolio
{
	public interface IPortfolioEnqueryService
	{
        Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request);
    }
}

