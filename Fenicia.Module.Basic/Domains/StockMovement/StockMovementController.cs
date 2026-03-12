using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;
using Fenicia.Module.Basic.Domains.StockMovement.Queries;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

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
    UpdateStockMovementHandler updateStockMovementHandler,
    GetStockMovementDashboardHandler getStockMovementDashboardHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetStockMovementResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetStockMovementResponse>>> GetAsync(
        [FromQuery] StockMovementQuery query,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var stockMovement =
            await getStockMovementHandler.Handle(new GetStockMovementQuery(query.StartDate,
                    query.EndDate,
                    query.Page,
                    query.PerPage),
                ct);

        return Ok(stockMovement);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddStockMovementResponse>> PostAsync(
        [FromBody] AddStockMovementCommand command,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var stockMovement = await addStockMovementHandler.Handle(command,
            ct);

        return new CreatedResult(string.Empty,
            stockMovement);
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
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var stockMovement = await updateStockMovementHandler.Handle(command with
            {
                Id = id
            },
            ct);

        return stockMovement is null ? NotFound() : new CreatedResult(string.Empty,
            stockMovement);
    }

    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockMovementDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<StockMovementDashboardResponse>> GetDashboardAsync(
        WideEventContext wide,
        [FromQuery] int days = 30,
        [FromQuery] int topLimit = 10,
        CancellationToken ct = default)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();
        
        var dashboard = await getStockMovementDashboardHandler.Handle(new GetStockMovementDashboardQuery(days,
                topLimit),
            ct);

        return Ok(dashboard);
    }

    public record StockMovementQuery(int Page, int PerPage)
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}
