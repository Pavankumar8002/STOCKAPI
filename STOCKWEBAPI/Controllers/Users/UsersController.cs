using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.Login;
using STOCKWEBAPI.DataEntities.Users;
using STOCKWEBAPI.ServiceInterface.Login;
using STOCKWEBAPI.ServiceInterface.Users;

namespace STOCKWEBAPI.Controllers.Users
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UsersController : ControllerBase
    {

        private readonly IUsersService _userService;

        public UsersController(IUsersService userService)
        {
            _userService = userService;
        }

        [HttpPost("AddUsers")]
        public async Task<IActionResult> SaveUsers(UsersRequestClass request)
        {
            var saveResult = await _userService.SaveUsers(request);
            return Ok(new { saveResult });
        }

    }
}

