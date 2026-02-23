using System.Net.Mime;

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
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ProductController(
    GetAllProductHandler getAllProductHandler,
    GetProductByIdHandler getProductByIdHandler,
    AddProductHandler addProductHandler,
    UpdateProductHandler updateProductHandler,
    DeleteProductHandler deleteProductHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<ProductResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var products = await getAllProductHandler.Handle(new GetAllProductQuery(page, perPage), ct);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var product = await getProductByIdHandler.Handle(new GetProductByIdQuery(id), ct);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ProductResponse>> PostAsync([FromBody] AddProductCommand command, CancellationToken ct)
    {
        var product = await addProductHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, product);
    }

    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ProductResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ProductResponse>> PatchAsync(
        [FromBody] UpdateProductCommand command,
        [FromRoute] Guid id,
        CancellationToken ct)
    {
        var product = await updateProductHandler.Handle(command with { Id = id }, ct);

        return product is null ? NotFound() : Ok(product);
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<ProductResponse>> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteProductHandler.Handle(new DeleteProductCommand(id), ct);

        return NoContent();
    }
}
