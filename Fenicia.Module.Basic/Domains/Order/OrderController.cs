using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.Order;

[ApiController]
[Route("[controller]")]
[Authorize]
public class OrderController(IOrderService orderService, IOrderDetailService orderDetailService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderRequest request, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        request.UserId = userId;
        var order = await orderService.AddAsync(request, ct);

        return new CreatedResult(string.Empty, order);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute] Guid id, CancellationToken ct)
    {
        var details = await orderDetailService.GetByOrderIdAsync(id, ct);

        return Ok(details);
    }
}