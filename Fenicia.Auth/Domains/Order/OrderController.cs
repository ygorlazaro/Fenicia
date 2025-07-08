namespace Fenicia.Auth.Domains.Order;

using System.Net.Mime;

using Common.Api;

using Data;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route(template: "[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(ILogger<OrderController> logger, IOrderService orderService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Creating new order for request {@Request}", request);

            var userId = ClaimReader.UserId(User);
            var companyId = ClaimReader.CompanyId(User);

            var order = await orderService.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

            if (order.Data is null)
            {
                logger.LogWarning(message: "Order creation failed: {Message}", order.Message);
                return StatusCode((int)order.Status, order.Message);
            }

            logger.LogInformation(message: "New order created successfully for user {UserId}", userId);
            return Ok(order.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Unexpected error while creating order");
            throw;
        }
    }
}
