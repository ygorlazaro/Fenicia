using Fenicia.Module.Basic.Domains.Inventory.GetInventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
public class InventoryController(
    GetInventoryHandler getInventoryHandler,
    GetInventoryByProductHandler getInventoryByProductHandler,
    GetInventoryByCategoryHandler getInventoryByCategoryHandler) : ControllerBase
{
    [HttpGet("/products/{productId:guid}")]
    public async Task<IActionResult> GetInventoryByProductIdAsync([FromRoute] Guid productId, CancellationToken ct)
    {
        var inventory = await getInventoryByProductHandler.Handle(new GetInventoryByProductQuery(productId, 1, 10), ct);

        return Ok(inventory);
    }

    [HttpGet("/category/{categoryId:guid}")]
    public async Task<IActionResult> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, CancellationToken ct)
    {
        var inventory = await getInventoryByCategoryHandler.Handle(new GetInventoryByCategoryQuery(categoryId, 1, 10), ct);

        return Ok(inventory);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryAsync([FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        var inventory = await getInventoryHandler.Handle(new GetInventoryQuery(page, perPage), ct);

        return Ok(inventory);
    }
}
