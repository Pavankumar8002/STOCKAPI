using System;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.RepositoryInterface.RootStackx;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Service.RootStackx
{
	public class EnquiryDetailsService : IEnquiryDetailsService
	{
        private readonly IEnquiryDetailsRepo _enqueryRepo;

        public EnquiryDetailsService(IEnquiryDetailsRepo enqueryRepo)
        {
            _enqueryRepo = enqueryRepo;
        }

        public async Task<string> ViewEnquiry()
        {
            return await _enqueryRepo.ViewEnquiry();
        }
    }
}

