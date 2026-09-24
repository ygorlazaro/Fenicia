using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Basic.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Inventory;

/// <summary>
/// Controller responsible for handling inventory-related operations.
/// </summary>
/// <param name="inventoryService">The inventory service.</param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class InventoryController(IInventoryService inventoryService) : ControllerBase
{
    /// <summary>
    /// Gets the inventory by product.
    /// </summary>
    /// <param name="productId">The product ID.</param>
    /// <param name="request">The inventory by product request.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Inventory data for the product.</returns>
    /// <response code="200">Inventory returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("product/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByProductIdAsync(
        [FromRoute] Guid productId,
        [FromQuery] InventoryByProductRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetByProductAsync(request with
            {
                ProductId = productId
            }, cancellationToken);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the inventory by category.
    /// </summary>
    /// <param name="categoryId">The category ID.</param>
    /// <param name="request">The inventory by category request.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Inventory data for the category.</returns>
    /// <response code="200">Inventory returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("category/{categoryId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryByCategoryIdAsync(
        [FromRoute] Guid categoryId,
        [FromQuery] InventoryByCategoryRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetByCategoryAsync(request with
            {
                CategoryId = categoryId
            }, cancellationToken);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the complete inventory with pagination and filtering.
    /// </summary>
    /// <param name="request">The inventory request with pagination and filters.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Inventory data.</returns>
    /// <response code="200">Inventory returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryResponse>> GetInventoryAsync(
        [FromQuery] InventoryRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var inventory = await inventoryService.GetAsync(request, cancellationToken);

            return Ok(inventory);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the inventory dashboard data.
    /// </summary>
    /// <param name="request">The inventory dashboard request.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Inventory dashboard data.</returns>
    /// <response code="200">Dashboard returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("dashboard")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryDashboardResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryDashboardResponse>> GetInventoryDashboardAsync(
        [FromQuery] InventoryDashboardRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var dashboard = await inventoryService.GetDashboardAsync(request, cancellationToken);

            return Ok(dashboard);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets the inventory health data.
    /// </summary>
    /// <param name="request">The inventory health request.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Inventory health data.</returns>
    /// <response code="200">Health returned successfully.</response>
    /// <response code="401">User not authorized.</response>
    [HttpGet("health")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(InventoryHealthResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<InventoryHealthResponse>> GetInventoryHealthAsync(
        [FromQuery] InventoryHealthRequest request,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var health = await inventoryService.GetHealthAsync(request, cancellationToken);

            return Ok(health);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
