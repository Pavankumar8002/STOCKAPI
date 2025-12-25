using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.Portfolio;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.ServiceInterface.Portfolio;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Controllers.Portfolio
{
    [Route("api/[controller]")]
    [ApiController]
    public class PortfolioEnqueryController : Controller
    {
        private readonly IPortfolioEnqueryService _enqueryService;

        public PortfolioEnqueryController(IPortfolioEnqueryService enqueryService)
        {
            _enqueryService = enqueryService;
        }

        [HttpPost("PostPortfolioEnquiry")]
        public async Task<IActionResult> SaveEnquiry(PortfolioEnqueryRequest request)
        {
            var saveResult = await _enqueryService.SavePortfolioEnquiry(request);
            return Ok(saveResult);
        }
    }
}

