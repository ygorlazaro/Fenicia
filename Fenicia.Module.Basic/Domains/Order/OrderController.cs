using Fenicia.Common.API;
using Fenicia.Common.Database.Requests.Basic;
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
    public async Task<IActionResult> CreateOrderAsync([FromBody] OrderRequest request, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        request.UserId = userId;
        var order = await orderService.AddAsync(request, cancellationToken);

        return new CreatedResult(string.Empty, order);
    }

    [HttpGet("{id:guid}/detail")]
    public async Task<IActionResult> GetDetailsAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var details = await orderDetailService.GetByOrderIdAsync(id, cancellationToken);

        return Ok(details);
    }
}
