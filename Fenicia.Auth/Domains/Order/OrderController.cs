namespace Fenicia.Auth.Domains.Order;

using System.Net.Mime;

using Common.API;
using Common.Database.Requests;
using Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController : ControllerBase
{
    private readonly ILogger<OrderController> logger;
    private readonly IOrderService orderService;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        this.logger = logger;
        this.orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Creating new order for request {@Request}", request);

            var userId = ClaimReader.UserId(User);
            var companyId = ClaimReader.CompanyId(User);

            var order = await orderService.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

            if (order.Data is null)
            {
                logger.LogWarning("Order creation failed: {Message}", order.Message);
                return StatusCode((int)order.Status, order.Message);
            }

            logger.LogInformation("New order created successfully for user {UserID}", userId);
            return Ok(order.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating order");
            throw;
        }
    }
}
