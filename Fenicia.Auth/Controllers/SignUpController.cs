using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace Fenicia.Auth.Controllers;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class SignUpController(ILogger<SignUpController> logger, IUserService userService) : ControllerBase
{
    /// <summary>
    /// Creates a new user account
    /// </summary>
    /// <param name="request">The user registration information</param>
    /// <response code="200">Returns the created user information</response>
    /// <response code="400">If the user creation fails</response>
    [HttpPost]
    [ProducesResponseType(typeof(UserResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<UserResponse>> CreateNewUserAsync(UserRequest request)
    {
        var user = await userService.CreateNewUserAsync(request);

        if (user is null)
        {
            return BadRequest(TextConstants.ThereWasAnErrorAtCreatingUser);
        }
        
        logger.LogInformation("New user - {email}", [request.Email]);

        return Ok(user);
    }
}