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
    private readonly ILogger<OrderController> _logger;
    private readonly IOrderService _orderService;

    public OrderController(ILogger<OrderController> logger, IOrderService orderService)
    {
        _logger = logger;
        _orderService = orderService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(OrderRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Creating new order for request {@Request}", request);

            var userId = ClaimReader.UserId(User);
            var companyId = ClaimReader.CompanyId(User);

            var order = await _orderService.CreateNewOrderAsync(userId, companyId, request, cancellationToken);

            if (order.Data is null)
            {
                _logger.LogWarning("Order creation failed: {Message}", order.Message);
                return StatusCode((int)order.Status, order.Message);
            }

            _logger.LogInformation("New order created successfully for user {UserID}", userId);
            return Ok(order.Data);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while creating order");
            throw;
        }
    }
}
