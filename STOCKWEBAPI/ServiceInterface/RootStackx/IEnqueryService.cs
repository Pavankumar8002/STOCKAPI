using System.Threading.Tasks;
using STOCKWEBAPI.DataEntities.RootStackx;

namespace STOCKWEBAPI.ServiceInterface.RootStackx
{
    public interface IEnqueryService
    {
        Task<dynamic> SaveEnquiry(EnquiryRequest request);
    }
}
