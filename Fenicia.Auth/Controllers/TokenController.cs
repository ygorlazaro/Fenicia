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
public class TokenController(ITokenService tokenService, IUserService userService, IUserRoleService userRoleService, ICompanyService companyService, ISubscriptionCreditService subscriptionCreditService) : ControllerBase
{
    /// <summary>
    /// Generates an authentication token for the user
    /// </summary>
    /// <param name="request">The token request containing credentials</param>
    /// <response code="200">Returns the authentication token</response>
    /// <response code="400">If the user has no roles assigned</response>
    /// <response code="404">If the company is not found</response>
    [HttpPost]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PostAsync(TokenRequest request)
    {
        var company = await companyService.GetByCnpjAsync(request.Cnpj);

        if (company is null)
        {
            throw new InvalidDataException(TextConstants.InvalidUsernameOrPassword);
        }
        
        var user = await userService.GetForLoginAsync(request);
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