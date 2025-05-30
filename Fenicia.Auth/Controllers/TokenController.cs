using Fenicia.Auth.Requests;
using Fenicia.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
public class TokenController(ITokenService tokenService, IUserService userService, IUserRoleService userRoleService, ICompanyService companyService): ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(TokenRequest request)
    {
        var user = await userService.GetByEmailAndCnpjAsync(request.Email, request.CNPJ);
        var company = await companyService.GetByCnpjAsync(request.CNPJ);

        if (user is null || company is null)
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
        
        var token = tokenService.GenerateToken(user, roles, company.Id);
        
        return Ok(token);
    }
}