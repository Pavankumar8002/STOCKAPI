using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/operations")]
public class OperationsController : ControllerBase
{
    private readonly ISteelCementService _s;
    public OperationsController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet("dashboard")]
    public async Task<IActionResult> Dashboard([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _s.GetDashboard(from, to));

    [HttpGet("outstanding")]
    public async Task<IActionResult> Outstanding()
        => Ok(await _s.GetOutstanding());

    [HttpGet("reports")]
    public async Task<IActionResult> Reports([FromQuery] DateTime? from, [FromQuery] DateTime? to)
        => Ok(await _s.GetReports(from, to));

    [HttpGet("settings")]
    public async Task<IActionResult> Settings()
        => Ok(await _s.GetSettings());
}