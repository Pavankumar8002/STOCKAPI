using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/debt")]
public class DebtController : ControllerBase
{
    private readonly ISteelCementService _s;
    public DebtController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet("outstanding")]
    public async Task<IActionResult> Outstanding() => Ok(await _s.GetOutstanding());
}