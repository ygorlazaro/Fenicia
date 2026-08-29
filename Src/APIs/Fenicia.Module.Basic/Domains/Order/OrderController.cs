using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

[ApiController]
[Route("[controller]")]
[Authorize]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(OrderService orderService, OrderDetailService orderDetailService) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Pagination<List<GetAllOrderResponse>>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Pagination<List<GetAllOrderResponse>>>> GetAsync(WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var orders = await orderService.GetAllAsync(new GetAllOrderQuery(page, perPage), ct);

            return Ok(orders);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetOrderByIdResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<GetOrderByIdResponse>> GetByIdAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var order = await orderService.GetByIdAsync(new GetOrderByIdQuery(id), ct);

            return order is null ? NotFound() : Ok(order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

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
            var order = await orderService.CreateAsync(command with { UserId = userId }, ClaimReader.UserId(User), ct);

            return new CreatedResult(string.Empty, order);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

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

            await orderService.DeleteAsync(new DeleteOrderCommand(id), ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetOrderDetailsByOrderIdResponse>))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<List<GetOrderDetailsByOrderIdResponse>>> GetDetailsAsync([FromRoute] Guid id, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var details = await orderDetailService.GetByOrderIdAsync(new GetOrderDetailsByOrderIdQuery(id), ct);

            return Ok(details);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpGet("analytics")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(OrderAnalyticsResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<OrderAnalyticsResponse>> GetAnalyticsAsync(WideEventContext wide, [FromQuery] int days = 90, [FromQuery] int topCustomersLimit = 10, CancellationToken ct = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var analytics = await orderService.GetAnalyticsAsync(new GetOrderAnalyticsQuery(days, topCustomersLimit), ct);

            return Ok(analytics);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
