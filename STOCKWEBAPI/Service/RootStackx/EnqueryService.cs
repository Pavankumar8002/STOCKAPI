using System;
using System.Threading.Tasks;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.DataEntities.Users;
using STOCKWEBAPI.RepositoryInterface.RootStackx;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Service.RootStackx
{
    public class EnqueryService : IEnqueryService
    {
        private readonly IEnqueryRepo _enqueryRepo;

        public EnqueryService(IEnqueryRepo enqueryRepo)
        {
            _enqueryRepo = enqueryRepo;
        }

        public async Task<dynamic> SaveEnquiry(EnquiryRequest request)
        {
            return await _enqueryRepo.SaveEnquiry(request);
        }
    }
}
