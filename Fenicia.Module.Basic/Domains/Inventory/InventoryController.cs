using System.Net.Mime;

using Fenicia.Module.Basic.Domains.Inventory.GetInventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InventoryController(
    GetInventoryHandler getInventoryHandler,
    GetInventoryByProductHandler getInventoryByProductHandler,
    GetInventoryByCategoryHandler getInventoryByCategoryHandler) : ControllerBase
{
    [HttpGet("/product/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByProductIdAsync([FromRoute] Guid productId, CancellationToken ct)
    {
        var inventory = await getInventoryByProductHandler.Handle(new GetInventoryByProductQuery(productId), ct);

        return Ok(inventory);
    }

    [HttpGet("/category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, CancellationToken ct)
    {
        var inventory = await getInventoryByCategoryHandler.Handle(new GetInventoryByCategoryQuery(categoryId), ct);

        return Ok(inventory);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var inventory = await getInventoryHandler.Handle(new GetInventoryQuery(page, perPage), ct);

        return Ok(inventory);
    }
}
