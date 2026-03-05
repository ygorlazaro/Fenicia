using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;
using Fenicia.Module.Basic.Domains.Order.Delete;
using Fenicia.Module.Basic.Domains.Order.GetAll;
using Fenicia.Module.Basic.Domains.Order.GetById;
using Fenicia.Module.Basic.Domains.Order.GetOrderAnalytics;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(
    GetAllOrderHandler getAllOrderHandler,
    GetOrderByIdHandler getOrderByIdHandler,
    CreateOrderHandler createOrderHandler,
    DeleteOrderHandler deleteOrderHandler,
    GetOrderDetailsByOrderIdHandler getOrderDetailsByOrderIdHandler,
    GetOrderAnalyticsHandler getOrderAnalyticsHandler) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllOrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetAllOrderResponse>>>> GetAsync(
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken ct = default)
    {
        var orders = await getAllOrderHandler.Handle(new GetAllOrderQuery(page, perPage), ct);

        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrderByIdResponse))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<GetOrderByIdResponse>> GetByIdAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var order = await getOrderByIdHandler.Handle(new GetOrderByIdQuery(id), ct);

        return order is null ? NotFound() : Ok(order);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateOrderResponse>> PostAsync([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var order = await createOrderHandler.Handle(command with { UserId = userId }, ct);

        return new CreatedResult(string.Empty, order);
    }

    [HttpDelete("{id:guid}")]
    [Authorize("Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync([FromRoute] Guid id, CancellationToken ct)
    {
        await deleteOrderHandler.Handle(new DeleteOrderCommand(id), ct);

        return NoContent();
    }

    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetOrderDetailsByOrderIdResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetOrderDetailsByOrderIdResponse>>> GetDetailsAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var details = await getOrderDetailsByOrderIdHandler.Handle(new GetOrderDetailsByOrderIdQuery(id), ct);

        return Ok(details);
    }

    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderAnalyticsResponse))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<OrderAnalyticsResponse>> GetAnalyticsAsync(
        [FromQuery] int days = 90,
        [FromQuery] int topCustomersLimit = 10,
        CancellationToken ct = default)
    {
        var analytics = await getOrderAnalyticsHandler.Handle(new GetOrderAnalyticsQuery(days, topCustomersLimit), ct);

        return Ok(analytics);
    }
}
