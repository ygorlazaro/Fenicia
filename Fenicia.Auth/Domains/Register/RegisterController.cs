using System.Net.Mime;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Register;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class RegisterController(IUserService userService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.UserId = request.Email;

        var userResponse = await userService.CreateNewUserAsync(request, cancellationToken);

        return Ok(userResponse);
    }
}
