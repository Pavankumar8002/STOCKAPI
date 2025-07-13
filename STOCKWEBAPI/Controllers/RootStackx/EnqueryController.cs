using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Controllers.RootStackx
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnqueryController : ControllerBase
    {
        private readonly IEnqueryService _enqueryService;

        public EnqueryController(IEnqueryService enqueryService)
        {
            _enqueryService = enqueryService;
        }

        [HttpPost("AddEnquiry")]
        public async Task<IActionResult> SaveEnquiry(EnquiryRequest request)
        {
            var saveResult = await _enqueryService.SaveEnquiry(request);
            return Ok(saveResult);
        }
    }
}
