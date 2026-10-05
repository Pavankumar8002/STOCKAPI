using Microsoft.AspNetCore.Mvc;
using STOCKWEBAPI.DataEntities.SteelCement;
using STOCKWEBAPI.ServiceInterface.SteelCement;

namespace STOCKWEBAPI.Controllers.SteelCement;
[ApiController, Route("api/products")]
public class ProductsController : ControllerBase
{
    private readonly ISteelCementService _s;
    public ProductsController(ISteelCementService s)=>_s=s;
    [HttpGet] public async Task<IActionResult> Get()=>Ok(await _s.GetProducts());
    [HttpGet("{id:guid}")] public async Task<IActionResult> Get(Guid id)=>(await _s.GetProduct(id)) is { } x?Ok(x):NotFound();
    [HttpPost] public async Task<IActionResult> Post(ProductDto x)=>Ok(await _s.SaveProduct(x));
    [HttpPut("{id:guid}")] public async Task<IActionResult> Put(Guid id,ProductDto x)=>Ok(await _s.UpdateProduct(id,x));
    [HttpDelete("{id:guid}")] public async Task<IActionResult> Delete(Guid id){await _s.DeleteProduct(id);return NoContent();}
}