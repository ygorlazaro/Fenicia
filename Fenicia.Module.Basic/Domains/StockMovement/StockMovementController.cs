using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Common.Exceptions;
using Fenicia.Module.Basic.Domains.StockMovement.Commands;
using Fenicia.Module.Basic.Domains.StockMovement.Handlers;
using Fenicia.Module.Basic.Domains.StockMovement.Queries;
using Fenicia.Module.Basic.Domains.StockMovement.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.StockMovement;

/// <summary>
///     Controller responsible for handling stock movement-related HTTP endpoints.
///     Provides endpoints to retrieve, create, and update stock movements, as well as dashboard analytics.
/// </summary>
/// <remarks>
///     Most endpoints require authentication. The Update endpoint requires Admin role.
/// </remarks>
[ApiController]
[Authorize]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StockMovementController(GetStockMovementHandler getStockMovementHandler, AddStockMovementHandler addStockMovementHandler, UpdateStockMovementHandler updateStockMovementHandler, GetStockMovementDashboardHandler getStockMovementDashboardHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of stock movements filtered by date range.
    /// </summary>
    /// <param name="query">Query parameters including pagination and date range.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A paginated list of stock movements.</returns>
    /// <response code="200">Returns the list of stock movements successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetStockMovementResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetStockMovementResponse>>> GetAsync([FromQuery] StockMovementQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(this.User).ToString();

            var stockMovement = await getStockMovementHandler.Handle(new GetStockMovementQuery(query.StartDate, query.EndDate, query.Page, query.PerPage), ct);

            return Ok(stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new stock movement entry.
    /// </summary>
    /// <param name="command">The command containing stock movement details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created stock movement with its details.</returns>
    /// <response code="201">Stock movement created successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddStockMovementResponse>> PostAsync([FromBody] AddStockMovementCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(this.User).ToString();

            var stockMovement = await addStockMovementHandler.Handle(command, ct);

            return new CreatedResult(string.Empty, stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Updates an existing stock movement (Admin only).
    /// </summary>
    /// <param name="id">The unique identifier of the stock movement to update.</param>
    /// <param name="command">The command containing updated stock movement details.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated stock movement if found, otherwise NotFound.</returns>
    /// <response code="200">Stock movement updated successfully.</response>
    /// <response code="400">Invalid request.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">User does not have Admin permission.</response>
    /// <response code="404">Stock movement not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPatch("{id:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UpdateStockMovementResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UpdateStockMovementResponse>> PatchAsync([FromRoute] Guid id, [FromBody] UpdateStockMovementCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(this.User).ToString();

            var stockMovement = await updateStockMovementHandler.Handle(command with { Id = id }, ct);

            return stockMovement is null ? NotFound() : Ok(stockMovement);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves stock movement dashboard analytics including totals, trends, and top products.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 30).</param>
    /// <param name="topLimit">Number of top products to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Dashboard analytics including totals, monthly data, and top products.</returns>
    /// <response code="200">Returns dashboard data successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(StockMovementDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<StockMovementDashboardResponse>> GetDashboardAsync(WideEventContext wide, [FromQuery] int days = 30, [FromQuery] int topLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(this.User).ToString();

            var dashboard = await getStockMovementDashboardHandler.Handle(new GetStockMovementDashboardQuery(days, topLimit), ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Query parameters for stock movement retrieval.
    /// </summary>
    public record StockMovementQuery(int Page, int PerPage)
    {
        public DateTime StartDate { get; set; }

        public DateTime EndDate { get; set; }
    }
}