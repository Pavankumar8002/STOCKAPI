using System;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.DataEntities.Users;

namespace STOCKWEBAPI.RepositoryInterface.RootStackx
{
	public interface IEnqueryRepo
	{
        Task<dynamic> SaveEnquiry(EnquiryRequest request);
    }
}

