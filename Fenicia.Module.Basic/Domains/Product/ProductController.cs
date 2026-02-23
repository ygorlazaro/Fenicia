using Fenicia.Module.Basic.Domains.Product.Add;
using Fenicia.Module.Basic.Domains.Product.Delete;
using Fenicia.Module.Basic.Domains.Product.GetAll;
using Fenicia.Module.Basic.Domains.Product.GetByCategoryId;
using Fenicia.Module.Basic.Domains.Product.GetById;
using Fenicia.Module.Basic.Domains.Product.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Product;

[Authorize]
[ApiController]
[Route("[controller]")]
public class ProductController(
    GetAllProductHandler getAllProductHandler,
    GetProductByIdHandler getProductByIdHandler,
    GetProductsByCategoryIdHandler getProductsByCategoryIdHandler,
    AddProductHandler addProductHandler,
    UpdateProductHandler updateProductHandler,
    DeleteProductHandler deleteProductHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var products = await getAllProductHandler.Handle(new GetAllProductQuery(page, perPage), ct);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var product = await getProductByIdHandler.Handle(new GetProductByIdQuery(id), ct);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddProductCommand command, CancellationToken ct)
    {
        var product = await addProductHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, product);
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> PatchAsync(
        [FromBody] UpdateProductCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var product = await updateProductHandler.Handle(command with { Id = id }, ct);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProductHandler.Handle(new DeleteProductCommand(id), ct);

        return NoContent();
    }
}
