using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;
[ApiController, Route("api/invoices")]
public class InvoicesController : ControllerBase
{
    private readonly ISteelCementService _s;
    public InvoicesController(ISteelCementService s)=>_s=s;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _s.GetInvoices());
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id)=>(await _s.GetInvoice(id)) is { } x?Ok(x):NotFound();
    [HttpPost] public async Task<IActionResult> Post(InvoiceDto x)=>Ok(await _s.CreateInvoice(x));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Put(Guid id,InvoiceDto x)=>Ok(await _s.UpdateInvoice(id,x));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id){await _s.CancelInvoice(id);return NoContent();}
    [HttpPost("{id:guid}/cancel")] public async Task<IActionResult> Cancel(Guid id){await _s.CancelInvoice(id);return NoContent();}
}