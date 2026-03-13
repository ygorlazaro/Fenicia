using System.Net.Mime;

using Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Handlers;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Responses;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(
    CreateNewOrderHandler createNewOrderHandler
    ) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewOrderResponse>> CreateNewOrderAsync(
        CreateNewOrderCommand request,
        [FromHeader] Headers headers,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = ClaimReader.UserId(this.User).ToString();

        var userId = ClaimReader.UserId(this.User);
        var companyId = headers.CompanyId;
        var command = new CreateNewOrderCommand(userId, companyId, request.Modules);
        var order = await createNewOrderHandler.Handle(command, ct);

        return order switch
        {
            null => BadRequest(),
            _ => Created(string.Empty, order)
        };
    }
}
