using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.RootStackx;
using STOCKWEBAPI.ServiceInterface.RootStackx;

namespace STOCKWEBAPI.Controllers.RootStackx
{
    [Route("api/[controller]")]
    [Authorize]
    public class EnquiryDetailsController : Controller
    {
        private readonly IEnquiryDetailsService _enqueryDetailsService;

        public EnquiryDetailsController(IEnquiryDetailsService enqueryDetailsService)
        {
            _enqueryDetailsService = enqueryDetailsService;
        }

        [HttpGet("ViewEnquiry")]
        public async Task<IActionResult> ViewEnquiry()
        {
            var viewResult = await _enqueryDetailsService.ViewEnquiry();
            return Ok(viewResult);
        }

    }
}

