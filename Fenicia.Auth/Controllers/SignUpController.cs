using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
public class SignUpController(IUserService userService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateNewUserAsync(NewUserRequest request)
    {
        var user = await userService.CreateNewUserAsync(request);

        if (user is null)
        {
            return BadRequest(TextConstants.ThereWasAnErrorAtCreating);
        }
        
        var response = new NewUserResponse
        {
            Name = user.Name,
            Email = user.Email,
            Id = user.Id
        };

        return Ok(response);
    }
}