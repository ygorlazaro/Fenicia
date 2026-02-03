using System.Net.Mime;

using Fenicia.Auth.Domains.User;
using Fenicia.Common.Api;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

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
        wide.Operation = "CreateNewUser";
        wide.UserId = request.Email;

        var userResponse = await userService.CreateNewUserAsync(request, cancellationToken);

        if (userResponse.Data is null)
        {
            return StatusCode((int)userResponse.Status, userResponse.Message);
        }

        return Ok(userResponse.Data);
    }
}
