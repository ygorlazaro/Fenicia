using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    [HttpGet("/products/{productId:guid}")]
    public async Task<IActionResult> GetInventoryByProductIdAsync([FromRoute] Guid productId, CancellationToken ct)
    {
        var inventory = await inventoryService.GetInventoryByProductAsync(productId, ct);

        return Ok(inventory);
    }

    [HttpGet("/category/{categoryId:guid}")]
    public async Task<IActionResult> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, CancellationToken ct)
    {
        var inventory = await inventoryService.GetInventoryByCategoryAsync(categoryId, ct);

        return Ok(inventory);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryAsync(CancellationToken ctctct)
    {
        var inventory = await inventoryService.GetInventoryAsync(ctctct);

        return Ok(inventory);
    }
}