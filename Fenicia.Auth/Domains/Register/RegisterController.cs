using System.Net.Mime;

using Fenicia.Auth.Domains.User.CreateNewUser;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewUserResponse>> CreateNewUserAsync(
        CreateNewUserQuery request,
        [FromServices] CreateNewUserHandler handler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.Email;

        var userResponse = await handler.Handle(request, ct);

        return Ok(userResponse);
    }
}
