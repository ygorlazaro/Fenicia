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
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetStockMovementResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetStockMovementResponse>>> GetAsync([FromQuery] StockMovementQuery query, CancellationToken ct)
    {
        var stockMovement =
            await getStockMovementHandler.Handle(new GetStockMovementQuery(query.StartDate, query.EndDate, query.Page, query.PerPage), ct);

        return Ok(stockMovement);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddStockMovementResponse>> PostAsync([FromBody] AddStockMovementCommand command, CancellationToken ct)
    {
        var stockMovement = await addStockMovementHandler.Handle(command, ct);

        return new CreatedResult(string.Empty, stockMovement);
    }

    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(UpdateStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateStockMovementResponse>> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] UpdateStockMovementCommand command,
        CancellationToken cancellationToken)
    {
        var stockMovement = await updateStockMovementHandler.Handle(command with { Id = id }, cancellationToken);

        return stockMovement is null ? NotFound() : new CreatedResult(string.Empty, stockMovement);
    }

    public record StockMovementQuery(int Page, int PerPage)
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
