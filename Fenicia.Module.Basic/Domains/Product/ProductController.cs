using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Product;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var products = await productService.GetAllAsync(ct);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var product = await productService.GetByIdAsync(id, ct);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ProductRequest request, CancellationToken ct)
    {
        var product = await productService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, product);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync(
        [FromBody] ProductRequest request,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var productRequest = await productService.UpdateAsync(request, ct);

        return productRequest is null ? NotFound() : Ok(productRequest);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await productService.DeleteAsync(id, ct);

        return NoContent();
    }
}