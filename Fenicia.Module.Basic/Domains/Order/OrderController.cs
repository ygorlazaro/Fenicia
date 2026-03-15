using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.Commands;
using Fenicia.Module.Basic.Domains.Order.Handlers;
using Fenicia.Module.Basic.Domains.Order.Queries;
using Fenicia.Module.Basic.Domains.Order.Responses;
using Fenicia.Module.Basic.Domains.OrderDetail.Handlers;
using Fenicia.Module.Basic.Domains.OrderDetail.Queries;
using Fenicia.Module.Basic.Domains.OrderDetail.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

/// <summary>
///     Controller responsible for handling order-related HTTP endpoints in the Basic module.
///     Provides full CRUD operations and analytics for orders.
/// </summary>
/// <remarks>
///     This controller manages product orders within a company. It provides:
///     - List all orders with pagination
///     - Get order by ID with full details
///     - Create new orders
///     - Delete orders (soft delete, Admin only)
///     - Get order details
///     - Get order analytics
///     Related documentation:
///     - See <see cref="Fenicia.Module.Basic.Domains.OrderDetail.Handlers.GetOrderDetailsByOrderIdHandler" /> for order details
///     - See <see cref="Fenicia.Module.Basic.Domains.Customer.CustomerController" /> for customer management
///     - See <see cref="Fenicia.Module.Basic.Domains.Employee.EmployeeController" /> for employee management
/// </remarks>
[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(GetAllOrderHandler getAllOrderHandler, GetOrderByIdHandler getOrderByIdHandler, CreateOrderHandler createOrderHandler, DeleteOrderHandler deleteOrderHandler, GetOrderDetailsByOrderIdHandler getOrderDetailsByOrderIdHandler, GetOrderAnalyticsHandler getOrderAnalyticsHandler) : ControllerBase
{
    /// <summary>
    ///     Retrieves a paginated list of all orders.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="page">Page number (default: 1).</param>
    /// <param name="perPage">Items per page (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Paginated list of orders.</returns>
    /// <response code="200">Returns the list of orders successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllOrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllOrderResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var orders = await getAllOrderHandler.Handle(new GetAllOrderQuery(page, perPage), ct);

            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves a specific order by its ID with full details.
    /// </summary>
    /// <param name="id">The order's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The order details or 404 if not found.</returns>
    /// <response code="200">Returns the order successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="404">Order not found.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrderByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetOrderByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var order = await getOrderByIdHandler.Handle(new GetOrderByIdQuery(id), ct);

            return order is null ? NotFound() : Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Creates a new order.
    /// </summary>
    /// <param name="command">The order creation command containing customer, items, and status.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created order or error response.</returns>
    /// <remarks>
    ///     This endpoint creates an order and automatically:
    ///     - Creates stock movement records for each item (reducing inventory)
    ///     - Updates product quantities
    /// </remarks>
    /// <response code="201">Order created successfully.</response>
    /// <response code="400">Invalid request data.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateOrderResponse>> PostAsync([FromBody] CreateOrderCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var userId = ClaimReader.UserId(User);
            var order = await createOrderHandler.Handle(command with { UserId = userId }, ct);

            return new CreatedResult(string.Empty, order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Deletes an order (soft delete).
    /// </summary>
    /// <param name="id">The order's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>No content on success.</returns>
    /// <remarks>
    ///     This endpoint performs a soft delete by setting the Deleted timestamp.
    ///     Requires Admin role to execute.
    /// </remarks>
    /// <response code="204">Order deleted successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <response code="403">User does not have Admin permission.</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpDelete("{id:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await deleteOrderHandler.Handle(new DeleteOrderCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves all details/items for a specific order.
    /// </summary>
    /// <param name="id">The order's unique identifier.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of order details including products, quantities, and prices.</returns>
    /// <response code="200">Returns the order details successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetOrderDetailsByOrderIdResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetOrderDetailsByOrderIdResponse>>> GetDetailsAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var details = await getOrderDetailsByOrderIdHandler.Handle(new GetOrderDetailsByOrderIdQuery(id), ct);

            return Ok(details);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    ///     Retrieves order analytics including sales trends, top customers, and order statistics.
    /// </summary>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="days">Number of days to analyze (default: 90).</param>
    /// <param name="topCustomersLimit">Number of top customers to return (default: 10).</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Comprehensive analytics data.</returns>
    /// <remarks>
    ///     Analytics include:
    ///     - Orders grouped by status with counts and totals
    ///     - Daily sales trends (order count, total value, items sold)
    ///     - Top customers by spending
    ///     - Average order value statistics (average, median, min, max)
    ///     - Recent cancelled orders
    /// </remarks>
    /// <response code="200">Returns analytics data successfully.</response>
    /// <response code="401">Unauthorized</response>
    /// <exception cref="UnauthorizedAccessException">User claim not found.</exception>
    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderAnalyticsResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderAnalyticsResponse>> GetAnalyticsAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topCustomersLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var analytics = await getOrderAnalyticsHandler.Handle(new GetOrderAnalyticsQuery(days, topCustomersLimit), ct);

            return Ok(analytics);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
