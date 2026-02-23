using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StockMovementController(IStockMovementService stockMovementService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] StockMovementQuery query, CancellationToken ct)
    {
        var stockMovimentation =
            await stockMovementService.GetMovementAsync(query.StartDate, query.EndDate, ct, query.Page, query.PerPage);

        return Ok(stockMovimentation);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] StockMovementRequest request, CancellationToken ct)
    {
        var stockMovimentation = await stockMovementService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, stockMovimentation);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] StockMovementRequest request,
        CancellationToken cancellationToken)
    {
        var stockMovimentation = await stockMovementService.UpdateAsync(id, request, cancellationToken);

        return stockMovimentation is null ? NotFound() : new CreatedResult(string.Empty, stockMovimentation);
    }

    public record StockMovementQuery(int Page, int PerPage)
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}