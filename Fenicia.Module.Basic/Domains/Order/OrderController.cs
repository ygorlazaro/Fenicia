using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrderController(
    CreateOrderHandler createOrderHandler,
    GetOrderDetailsByOrderIdHandler getOrderDetailsByOrderIdHandler) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] CreateOrderCommand command, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var order = await createOrderHandler.Handle(command with { UserId = userId }, ct);

        return new CreatedResult(string.Empty, order);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var details = await getOrderDetailsByOrderIdHandler.Handle(new GetOrderDetailsByOrderIdQuery(id), ct);

        return Ok(details);
    }
}
