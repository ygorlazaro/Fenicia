using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;
using Fenicia.Module.Basic.Domains.Inventory.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

/// <summary>
///     Controller responsible for handling inventory-related HTTP endpoints.
///     Provides endpoints for inventory management, dashboards, and health checks.
/// </summary>
/// <remarks>
///     All endpoints require authentication.
/// </remarks>
[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InventoryController(GetInventoryHandler getInventoryHandler, GetInventoryByProductHandler getInventoryByProductHandler, GetInventoryByCategoryHandler getInventoryByCategoryHandler, GetInventoryDashboardHandler getInventoryDashboardHandler, GetInventoryHealthHandler getInventoryHealthHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves inventory data for a specific product.
    /// </summary>
    /// <param name="productId">Product's unique identifier.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <returns>Inventory details for the specified product.</returns>
    /// <response code="200">Inventory retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByProductIdAsync([FromRoute] Guid productId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await getInventoryByProductHandler.Handle(new GetInventoryByProductQuery(productId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves inventory data for products in a specific category.
    /// </summary>
    /// <param name="categoryId">Category's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory details for products in the specified category.</returns>
    /// <response code="200">Inventory retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByCategoryIdAsync([FromRoute] Guid categoryId, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await getInventoryByCategoryHandler.Handle(new GetInventoryByCategoryQuery(categoryId), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves paginated inventory data for all products.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated inventory list.</returns>
    /// <response code="200">Inventory retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await getInventoryHandler.Handle(new GetInventoryQuery(page, perPage), ct);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves inventory dashboard with overview metrics.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory dashboard with key metrics.</returns>
    /// <response code="200">Dashboard retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryDashboardResponse>> GetInventoryDashboardAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await getInventoryDashboardHandler.Handle(ct);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves inventory health analysis including overstock and zero-movement alerts.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="zeroMovementDays">Days threshold for zero movement detection (default: 90).</param>
    /// <param name="overstockMultiplier">Multiplier for overstock calculation (default: 3.0).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Inventory health analysis.</returns>
    /// <response code="200">Health analysis retrieved successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryHealthResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<InventoryHealthResponse>> GetInventoryHealthAsync(WideEventContext wide, [FromQuery] int zeroMovementDays = 90, [FromQuery] double overstockMultiplier = 3.0, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var health = await getInventoryHealthHandler.Handle(new GetInventoryHealthQuery(zeroMovementDays, overstockMultiplier), ct);

            return Ok(health);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
