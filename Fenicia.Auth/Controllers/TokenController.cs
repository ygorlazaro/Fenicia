using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Controllers;

[AllowAnonymous]
[Route("[controller]")]
[ApiController]
public class TokenController(ITokenService tokenService, IUserService userService, IUserRoleService userRoleService, ICompanyService companyService, ISubscriptionCreditService subscriptionCreditService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> PostAsync(TokenRequest request)
    {
        var user = await userService.GetByEmailAndCnpjAsync(request.Email, request.Cnpj);
        var company = await companyService.GetByCnpjAsync(request.Cnpj);

        if (user is null || company is null)
        {
            return BadRequest(TextConstants.InvalidUsernameOrPassword);
        }

        var isValidPassword = userService.ValidatePasswordAsync(request.Password, user.Password);

        if (!isValidPassword)
        {
            return BadRequest(TextConstants.InvalidUsernameOrPassword);
        }

        var roles = await userRoleService.GetRolesByUserAsync(user.Id);

        if (roles.Length == 0)
        {
            return BadRequest(TextConstants.UserWithoutRoles);
        }

        var modules = await subscriptionCreditService.GetActiveModulesTypesAsync(company.Id);
        var token = tokenService.GenerateToken(user, roles, company.Id, modules);

        return Ok(new TokenResponse
        {
            Token = token
        });
    }
}