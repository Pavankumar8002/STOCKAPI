using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.Login;
using STOCKWEBAPI.Helpers;
using STOCKWEBAPI.ServiceInterface.Login;
using System.Threading.Tasks;
using System.Dynamic;

namespace STOCKWEBAPI.Controllers.Login
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {
        private readonly ILoginService _loginService;
        private readonly JwtTokenGenerator _jwtTokenGenerator;

        public LoginController(ILoginService loginService, JwtTokenGenerator jwtTokenGenerator)
        {
            _loginService = loginService;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            dynamic authResult = await _loginService.Authenticate(request.Username, request.Password);
            if (authResult.status == 1)
            {
                authResult.token = _jwtTokenGenerator.GenerateToken(request.Username);
                return Ok(authResult);
            }
            return Unauthorized(authResult);
        }
    }
}
