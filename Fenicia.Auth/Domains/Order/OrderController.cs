using System.Net.Mime;

using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Order.Logic;
using Fenicia.Common.Api;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Controller responsible for managing order-related operations
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(
    ILogger<OrderController> logger,
    IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Creates a new order for the authenticated user's company
    /// </summary>
    /// <param name="request">The order creation details</param>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>The created order response</returns>
    /// <response code="200">Returns the created order</response>
    /// <response code="401">If the user is not authenticated</response>
    /// <response code="500">If an unexpected error occurs</response>
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

            logger.LogInformation("New order created successfully for user {UserId}", userId);
            return Ok(order.Data);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error while creating order");
            throw;
        }
    }
}
