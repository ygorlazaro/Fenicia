using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;
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
    CreateOrderHandler createOrderHandler,
    GetOrderDetailsByOrderIdHandler getOrderDetailsByOrderIdHandler) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateOrderResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateOrderResponse>> CreateOrderAsync([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var order = await createOrderHandler.Handle(command with { UserId = userId }, ct);

        return new CreatedResult(string.Empty, order);
    }

    [HttpGet("{id:guid}/detail")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetOrderDetailsByOrderIdResponse>))]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetOrderDetailsByOrderIdResponse>>> GetDetailsAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var details = await getOrderDetailsByOrderIdHandler.Handle(new GetOrderDetailsByOrderIdQuery(id), ct);

        return Ok(details);
    }
}
