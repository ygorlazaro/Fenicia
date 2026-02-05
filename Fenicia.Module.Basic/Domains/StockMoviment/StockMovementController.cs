using Fenicia.Common;
using Fenicia.Common.Data.Requests.Basic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMoviment;

[ApiController]
[Authorize]
[Route("[controller]")]
public class StockMovementController(IStockMovementService stockMovementService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAsync([FromQuery] StockMovementQuery query, CancellationToken cancellationToken)
    {
        var stockMovimentation = await stockMovementService.GetMovementAsync(query.StartDate, query.EndDate, cancellationToken, query.Page, query.PerPage);

        return Ok(stockMovimentation);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] StockMovementRequest request, CancellationToken cancellationToken)
    {
        var stockMovimentation = await stockMovementService.AddAsync(request, cancellationToken);

        return new CreatedResult(string.Empty, stockMovimentation);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync([FromRoute] Guid id, [FromBody] StockMovementRequest request, CancellationToken cancellationToken)
    {
        var stockMovimentation = await stockMovementService.UpdateAsync(id, request, cancellationToken);

        if (stockMovimentation is null)
        {
            return NotFound();
        }

        return new CreatedResult(string.Empty, stockMovimentation);
    }

    public class StockMovementQuery : PaginationQuery
    {
        public DateTime StartDate
        {
            get; set;
        }

        public DateTime EndDate
        {
            get; set;
        }
    }
}
