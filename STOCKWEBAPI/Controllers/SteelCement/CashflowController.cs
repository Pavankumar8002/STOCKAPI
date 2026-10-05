using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/cashflow")]
public class CashflowController : ControllerBase
{
    private readonly ISteelCementService _s;
    public CashflowController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet]
    public async Task<IActionResult> Get()
        => Ok(await _s.GetPayments());

    [HttpPost]
    public async Task<IActionResult> Post(PaymentDto request)
        => Ok(await _s.RecordPayment(request));
}