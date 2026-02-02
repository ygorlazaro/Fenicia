using System.Net.Mime;

using Fenicia.Common.API;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(ILogger<OrderController> logger, IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request, [FromHeader] Headers headers, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating new order for request {@Request}", request);

            var userId = ClaimReader.UserId(this.User);
            var companyId = headers.CompanyId;
            var order = await orderService.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

            if (order.Data is null)
            {
                logger.LogWarning("Order creation failed: {Message}", order.Message);
                return this.StatusCode((int)order.Status, order.Message);
            }

            logger.LogInformation("New order created successfully for user {UserID}", userId);

            return this.Ok(order.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating order");

            throw;
        }
    }
}
