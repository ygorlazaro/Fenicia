using Fenicia.Common;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Module.Basic.Domains.Product;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductCategoryController(IProductCategoryService productCategoryService, IProductService productService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync(CancellationToken ct)
    {
        var productCategory = await productCategoryService.GetAllAsync(ct);

        return Ok(productCategory);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var productCategory = await productCategoryService.GetByIdAsync(id, ct);

        if (productCategory is null)
        {
            return NotFound();
        }

        return Ok(productCategory);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] ProductCategoryRequest request, CancellationToken ct)
    {
        var productCategory = await productCategoryService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, productCategory);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync([FromBody] ProductCategoryRequest request, [FromRoute] Guid id, CancellationToken ct)
    {
        var productCategory = await productCategoryService.UpdateAsync(request, ct);

        if (productCategory is null)
        {
            return NotFound();
        }

        return Ok(productCategory);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await productCategoryService.DeleteAsync(id, ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/product")]
    public async Task<IActionResult> GetProductsByCategoryAsync([FromRoute] Guid categoryId, [FromQuery] PaginationQuery query, CancellationToken ct)
    {
        var products = await productService.GetByCategoryIdAsync(categoryId, ct, query.Page, query.PerPage);

        return Ok(products);
    }
}
