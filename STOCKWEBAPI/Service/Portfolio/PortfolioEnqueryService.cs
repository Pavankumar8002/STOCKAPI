using System;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.RepositoryInterface.Portfolio;
using STOCKWEBAPI.RepositoryInterface.RootStackx;
using STOCKWEBAPI.ServiceInterface.Portfolio;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Service.Portfolio
{
	public class PortfolioEnqueryService : IPortfolioEnqueryService
    {
            private readonly IPortfolioEnqueryRepo _enqueryRepo;

            public PortfolioEnqueryService(IPortfolioEnqueryRepo enqueryRepo)
            {
                _enqueryRepo = enqueryRepo;
            }

            public async Task<dynamic> SavePortfolioEnquiry(PortfolioEnqueryRequest request)
            {
                return await _enqueryRepo.SavePortfolioEnquiry(request);
            }
        }
    }