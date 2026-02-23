using Fenicia.Module.Basic.Domains.StockMovement.Add;
using Fenicia.Module.Basic.Domains.StockMovement.GetMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StockMovementController(
    GetStockMovementHandler getStockMovementHandler,
    AddStockMovementHandler addStockMovementHandler,
    UpdateStockMovementHandler updateStockMovementHandler) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] StockMovementQuery query, CancellationToken ct)
    {
        var stockMovimentation =
            await getStockMovementHandler.Handle(new GetStockMovementQuery(query.StartDate, query.EndDate, query.Page, query.PerPage), ct);

        return Ok(stockMovimentation);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovimentation = await addStockMovementHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, stockMovimentation);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateStockMovementCommand command,
        CancellationToken cancellationToken)
    {
        var stockMovimentation = await updateStockMovementHandler.Handle(command with { Id = id }, cancellationToken);

        return stockMovimentation is null ? NotFound() : new CreatedResult(string.Empty, stockMovimentation);
    }

    public record StockMovementQuery(int Page, int PerPage)
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
