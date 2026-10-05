using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/inventory")]
public class InventoryController : ControllerBase
{
    private readonly ISteelCementService _s;
    public InventoryController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _s.GetProducts());
}