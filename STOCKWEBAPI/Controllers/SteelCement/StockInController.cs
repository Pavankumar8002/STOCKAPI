using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/stock-in")]
public class StockInController : ControllerBase
{
    private readonly ISteelCementService _s;
    public StockInController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _s.GetPurchases());

    [HttpPost]
    public async Task<IActionResult> Post(PurchaseDto request)
        => Ok(await _s.CreatePurchase(request));
}