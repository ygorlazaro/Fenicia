using System.Net.Mime;

using Fenicia.Module.Basic.Domains.StockMovement.Add;
using Fenicia.Module.Basic.Domains.StockMovement.GetMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Update;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMovement;

[ApiController]
[Authorize]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StockMovementController(
    GetStockMovementHandler getStockMovementHandler,
    AddStockMovementHandler addStockMovementHandler,
    UpdateStockMovementHandler updateStockMovementHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<StockMovementResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StockMovementResponse>> GetAsync([FromQuery] StockMovementQuery query, CancellationToken ct)
    {
        var stockMovimentation =
            await getStockMovementHandler.Handle(new GetStockMovementQuery(query.StartDate, query.EndDate, query.Page, query.PerPage), ct);

        return Ok(stockMovimentation);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<StockMovementResponse>> PostAsync([FromBody] AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovimentation = await addStockMovementHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, stockMovimentation);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(StockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<StockMovementResponse>> PatchAsync(
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
