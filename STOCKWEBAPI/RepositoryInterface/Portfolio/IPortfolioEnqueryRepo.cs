using System;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.DataEntities.RootStackx;

namespace STOCKWEBAPI.RepositoryInterface.Portfolio
{
	public interface IPortfolioEnqueryRepo
	{
        Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request);
    }
}

