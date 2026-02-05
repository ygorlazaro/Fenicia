using Fenicia.Common.Database.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Product;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken cancellationToken)
    {
        var products = await productService.GetAllAsync(cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var product = await productService.GetByIdAsync(id, cancellationToken);

        if (product is null)
        {
            return NotFound();
        }

        return Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ProductRequest request, CancellationToken cancellationToken)
    {
        var product = await productService.AddAsync(request, cancellationToken);

        return new CreatedResult(string.Empty, product);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] ProductRequest request, [FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var productRequest = await productService.UpdateAsync(request, cancellationToken);

        if (productRequest is null)
        {
            return NotFound();
        }

        return Ok(productRequest);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        await productService.DeleteAsync(id, cancellationToken);

        return NoContent();
    }
}
