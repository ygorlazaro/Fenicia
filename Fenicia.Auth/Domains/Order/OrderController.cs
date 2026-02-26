using System.Net.Mime;

using Fenicia.Auth.Domains.Order.CreateNewOrder;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewOrderResponse>> CreateNewOrderAsync(
        CreateNewOrderCommand request,
        [FromHeader] Headers headers,
        [FromServices] CreateNewOrderHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;
        var order = await handler.Handle(new CreateNewOrderCommand(userId, companyId, request.Modules), ct);

        return order is null ? (ActionResult<CreateNewOrderResponse>)BadRequest() : (ActionResult<CreateNewOrderResponse>)Ok(order);
    }
}
