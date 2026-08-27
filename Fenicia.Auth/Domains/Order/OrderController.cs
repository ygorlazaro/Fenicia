using System.Net.Mime;

using Fenicia.Auth.Domains.Order.Command;
using Fenicia.Auth.Domains.Order.Response;
using Fenicia.Auth.Domains.Order;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Order;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class OrderController(OrderService orderService) : ControllerBase
{

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewOrderResponse>> CreateNewOrderAsync(CreateNewOrderCommand request, [FromHeader] Headers headers, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var userId = ClaimReader.UserId(User);
            var companyId = headers.CompanyId;
            var command = new CreateNewOrderCommand(userId, companyId, request.Modules);
            var order = await orderService.CreateAsync(command, ct);

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
        catch (ItemNotExistsException ex)
        {
            return NotFound(new { Message = ex.Message });
        }
    }
}
