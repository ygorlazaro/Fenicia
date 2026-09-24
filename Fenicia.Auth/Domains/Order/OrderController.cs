using System.Net.Mime;
using Fenicia.Auth.Domains.Order.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.Order;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

/// <summary>
/// Controller for managing order operations.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(IOrderService orderService) : ControllerBase
{
    /// <summary>
    /// Creates a new order with the specified modules.
    /// </summary>
    /// <param name="request">Command with list of module IDs</param>
    /// <param name="companyId">Company ID from header</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Created order data</returns>
    /// <response code="201">Order created successfully</response>
    /// <response code="400">Invalid request (e.g., modules not found)</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="403">User not associated with the company</response>
    /// <response code="404">Company or modules not found</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<OrderResponse>> CreateNewOrderAsync(
        OrderRequest request,
        [FromHeader(Name = "CompanyId")] Guid companyId,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            request.UserId = userId;
            var order = await orderService.CreateAsync(request, cancellationToken);

            return order switch
            {
                null => BadRequest(),
                _ => Created(string.Empty, order)
            };
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (PermissionDeniedException)
        {
            return Forbid();
        }
        catch (ForbiddenException ex)
        {
            return NotFound(new { ex.Message });
        }
    }
}
