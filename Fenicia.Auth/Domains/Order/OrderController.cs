using System.Net.Mime;

using Fenicia.Auth.Domains.Order.CreateNewOrder.Commands;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Handlers;
using Fenicia.Auth.Domains.Order.CreateNewOrder.Responses;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
///     Controller responsible for handling order-related HTTP endpoints in the Auth module.
///     Provides endpoints for creating new orders (module subscriptions).
/// </summary>
/// <remarks>
///     This controller handles the creation of orders for module subscriptions.
///     It integrates with the subscription system to automatically create subscriptions
///     and credits when an order is placed. See <see cref="CreateNewOrderHandler" /> for details.
/// </remarks>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(CreateNewOrderHandler createNewOrderHandler) : ControllerBase
{
    /// <summary>
    ///     Creates a new order for module subscriptions.
    /// </summary>
    /// <param name="request">The command containing user ID, company ID, and list of module IDs to subscribe.</param>
    /// <param name="headers">HTTP headers containing company context.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The created order ID or error response.</returns>
    /// <remarks>
    ///     This endpoint:
    ///     1. Validates the user belongs to the company
    ///     2. Validates the requested modules exist
    ///     3. Automatically adds Basic module if not included
    ///     4. Creates the order with Approved status
    ///     5. Creates a 1-month subscription with credits for each module
    /// </remarks>
    /// <response code="201">Order created successfully.</response>
    /// <response code="400">Invalid request or modules not found.</response>
    /// <response code="500">Internal server error.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewOrderResponse>> CreateNewOrderAsync(CreateNewOrderCommand request, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
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