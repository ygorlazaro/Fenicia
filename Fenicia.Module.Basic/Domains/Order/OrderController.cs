using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.Basic.Order;
using Fenicia.Module.Basic.Domains.Order.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

/// <summary>
/// Controller responsible for handling order-related operations.
/// </summary>
/// <param name="orderService">The order service.</param>
/// <param name="companyContext">The company context.</param>
[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(IOrderService orderService, ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    /// Gets a paginated list of orders.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="query">The pagination and filter query.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated list of orders.</returns>
    /// <response code="200">List of orders returned successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<OrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<OrderResponse>>>> GetAsync(
        WideEventContext wide,
        [FromQuery] OrderQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var orders = await orderService.GetAllAsync(query, cancellationToken);

            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets an order by ID.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Order details.</returns>
    /// <response code="200">Order found.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="404">Order not found.</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var order = await orderService.GetByIdAsync(id, cancellationToken);

            return order is null ? NotFound() : Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets order details by order ID.
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Order details.</returns>
    /// <response code="200">Details returned successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("{id:guid}/orderdetail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<OrderDetailResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<OrderDetailResponse>>> GetOrderDetailsAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var details = await orderService.GetDetailsByOrderIdAsync(id, cancellationToken);

            return Ok(details);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Creates a new order.
    /// </summary>
    /// <param name="command">Order data.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Created order.</returns>
    /// <response code="201">Order created successfully.</response>
    /// <response code="400">Invalid data.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(OrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> PostAsync(
        [FromBody] OrderRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var userId = ClaimReader.UserId(User);
            command.UserId = userId;
            var order = await orderService.CreateAsync(
                command,
                companyContext.CompanyId,
                cancellationToken);

            return CreatedAtAction(nameof(GetByIdAsync), new { id = order.Id }, order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Deletes an order (soft delete).
    /// </summary>
    /// <param name="id">Order ID.</param>
    /// <param name="wide">Wide event context.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="204">Order deleted successfully.</response>
    /// <response code="401">User not authenticated.</response>
    /// <response code="403">Access denied.</response>
    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin,God")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await orderService.DeleteAsync(
                id,
                companyContext.CompanyId,
                cancellationToken);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Gets order analytics.
    /// </summary>
    /// <param name="wide">Wide event context.</param>
    /// <param name="request">The analytics request parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Order analytics.</returns>
    /// <response code="200">Analytics returned successfully.</response>
    /// <response code="401">User not authenticated.</response>
    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderAnalyticsResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderAnalyticsResponse>> GetAnalyticsAsync(
        WideEventContext wide,
        [FromQuery] OrderAnalyticsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var analytics = await orderService.GetAnalyticsAsync(request, cancellationToken);

            return Ok(analytics);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}