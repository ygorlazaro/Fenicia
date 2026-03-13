using System.Net.Mime;

using Fenicia.Auth.Domains.User.Commands;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Auth.Domains.User.Responses;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(
    CreateNewUserHandler createNewUserHandler
    ) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<CreateNewUserResponse>> CreateNewUserAsync(
        CreateNewUserCommand request,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.Email;

        var userResponse = await createNewUserHandler.Handle(request, ct);

        return Created(string.Empty, userResponse);
    }
}
