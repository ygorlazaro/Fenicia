using Fenicia.Common;
using Fenicia.Module.Basic.Domains.Product.GetAll;
using Fenicia.Module.Basic.Domains.Product.GetByCategoryId;
using Fenicia.Module.Basic.Domains.ProductCategory.Add;
using Fenicia.Module.Basic.Domains.ProductCategory.Delete;
using Fenicia.Module.Basic.Domains.ProductCategory.GetAll;
using Fenicia.Module.Basic.Domains.ProductCategory.GetById;
using Fenicia.Module.Basic.Domains.ProductCategory.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.ProductCategory;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductCategoryController(
    GetAllProductCategoryHandler getAllProductCategoryHandler,
    GetProductCategoryByIdHandler getProductCategoryByIdHandler,
    AddProductCategoryHandler addProductCategoryHandler,
    UpdateProductCategoryHandler updateProductCategoryHandler,
    DeleteProductCategoryHandler deleteProductCategoryHandler,
    GetProductsByCategoryIdHandler getProductsByCategoryIdHandler) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<ProductCategoryResponse>> GetAsync(CancellationToken ct)
    {
        var productCategory = await getAllProductCategoryHandler.Handle(new GetAllProductCategoryQuery(), ct);

        return Ok(productCategory);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProductCategoryResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var productCategory = await getProductCategoryByIdHandler.Handle(new GetProductCategoryByIdQuery(id), ct);

        return productCategory is null ? NotFound() : Ok(productCategory);
    }

    [HttpPost]
    public async Task<ActionResult<ProductCategoryResponse>> PostAsync([FromBody] AddProductCategoryCommand command, CancellationToken ct)
    {
        var productCategory = await addProductCategoryHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, productCategory);
    }

    [HttpPatch("{id:guid}")]
    public async Task<ActionResult<ProductCategoryResponse>> PatchAsync(
        [FromBody] UpdateProductCategoryCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var productCategory = await updateProductCategoryHandler.Handle(command with { Id = id }, ct);

        return productCategory is null ? NotFound() : Ok(productCategory);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<ProductCategoryResponse>> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProductCategoryHandler.Handle(new DeleteProductCategoryCommand(id), ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/product")]
    public async Task<ActionResult<ProductCategoryResponse>> GetProductsByCategoryAsync(
        [FromRoute] Guid categoryId,
        [FromQuery] PaginationQuery query,
        CancellationToken ct)
    {
        var products = await getProductsByCategoryIdHandler.Handle(new GetProductsByCategoryIdQuery(categoryId, query.Page, query.PerPage), ct);

        return Ok(products);
    }
}
