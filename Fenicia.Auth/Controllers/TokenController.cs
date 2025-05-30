using Fenicia.Auth.Requests;
using Fenicia.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
public class TokenController(ITokenService tokenService, IUserService userService, IUserRoleService userRoleService): ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(TokenRequest request)
    {
        var user = await userService.GetByEmailAsync(request.Email);

        if (user is null)
        {
            return BadRequest(TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = await userService.ValidatePasswordAsync(request.Password, user.Password);

        if (!isValidPassword)
        {
            return BadRequest(TextConstants.InvalidUsernameOrPassword);
        }
        
        var roles = await userRoleService.GetRolesByUserAsync(user.Id);

        if (roles.Length == 0)
        {
            return BadRequest(TextConstants.UserWithoutRoles);
        }
        
        var token = tokenService.GenerateToken(user, roles);
        
        return Ok(token);
    }
}