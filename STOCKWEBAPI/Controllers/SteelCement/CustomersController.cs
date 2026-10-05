using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;
[ApiController, Route("api/customers")]
public class CustomersController : ControllerBase
{
    private readonly ISteelCementService _s;
    public CustomersController(ISteelCementService s)=>_s=s;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _s.GetCustomers());
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id)=> (await _s.GetCustomer(id)) is { } x ? Ok(x) : NotFound();
    [HttpPost] public async Task<IActionResult> Post(CustomerDto x)=>Ok(await _s.SaveCustomer(x));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Put(Guid id,CustomerDto x)=>Ok(await _s.UpdateCustomer(id,x));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id){await _s.DeleteCustomer(id);return NoContent();}
}