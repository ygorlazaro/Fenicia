using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

[ApiController]
[Route("[controller]")]
[Authorize]
public class InventoryController(IInventoryService inventoryService): ControllerBase
{
    [HttpGet("/products/{productId}")]
    public async Task<IActionResult> GetInventoryByProductIdAsync([FromRoute] Guid productId, CancellationToken cancellationToken)
    {
        var inventory = await inventoryService.GetInventoryByProductAsync(productId, cancellationToken);

        return Ok(inventory);
    }

    [HttpGet("/category/{categoryId}")]
    public async Task<IActionResult> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, CancellationToken cancellationToken)
    {
        var inventory = await inventoryService.GetInventoryByCategoryAsync(categoryId, cancellationToken);

        return Ok(inventory);
    }

    [HttpGet]
    public async Task<IActionResult> GetInventoryAsync(CancellationToken cancellationToken)
    {
        var inventory = await inventoryService.GetInventoryAsync(cancellationToken);

        return Ok(inventory);
    }
}
