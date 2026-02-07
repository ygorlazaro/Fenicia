using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var userId = ClaimReader.UserId(User);
        var companyId = headers.CompanyId;
        var order = await orderService.CreateNewOrderAsync(userId, companyId, request, ct);

        return Ok(order);
    }
}
