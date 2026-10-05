using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.Repository.SteelCement;
using STOCKWEBAPI.Service.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;

[ApiController, Route("api/settings")]
public class SettingsController : ControllerBase
{
    private readonly ISteelCementService _s;
    public SettingsController(IConfiguration configuration, ISteelCementService? service = null)
        => _s = service ?? new SteelCementService(new SteelCementRepository(configuration));

    [HttpGet]
    public async Task<IActionResult> Get() => Ok(await _s.GetSettings());

    [HttpPut]
    public async Task<IActionResult> Put(ShopSettingsDto request) => Ok(await _s.UpdateSettings(request));
}