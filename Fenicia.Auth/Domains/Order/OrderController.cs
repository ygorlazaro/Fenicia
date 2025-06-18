using System.Net.Mime;

using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(ILogger<OrderController> logger, IOrderService orderService)
    : ControllerBase
{
    /// <summary>
    /// Creates a new order for the authenticated user's company
    /// </summary>
    /// <param name="request">The order creation details</param>
    /// <response code="200">Returns the created order</response>
    /// <response code="401">If the user is not authenticated</response>
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request)
    {
        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);

        var order = await orderService.CreateNewOrderAsync(userId, companyId, request);

        if (order.Data is null)
        {
            return StatusCode((int)order.StatusCode, order.Message);
        }

        logger.LogInformation("New order made - {userId}", [userId]);

        return Ok(order.Data);
    }
}
