using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;
using Fenicia.Common.Api;

namespace Fenicia.Auth.Controllers;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(
    ILogger<TokenController> logger,
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUserService userService,
    IUserRoleService userRoleService,
    ICompanyService companyService,
    ISubscriptionCreditService subscriptionCreditService) : ControllerBase
{
    /// <summary>
    /// Generates an authentication token for the user
    /// </summary>
    /// <param name="request">The token request containing credentials</param>
    /// <response code="200">Returns the authentication token</response>
    /// <response code="400">If the user has no roles assigned</response>
    /// <response code="404">If the company is not found</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(TokenRequest request)
    {
        var company = await companyService.GetByCnpjAsync(request.Cnpj);

        if (company is null)
        {
            logger.LogInformation("Invalid login - {email}", [request.Email]);
            throw new InvalidDataException(TextConstants.InvalidUsernameOrPassword);
        }

        var user = await userService.GetForLoginAsync(request);
        var response = await PopulateTokenAsync(user, company.Id);

        return response;
    }

    /// <summary>
    /// Gemerate a new authentication token for the user
    /// </summary>
    /// <param name="request">The refresh token request containing the refresh token</param>
    /// <response code="200">Returns the authentication token</response>
    [HttpPost]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request)
    {
        logger.LogInformation("Refreshing token");

        var userId = ClaimReader.UserId(User);
        var companyId = ClaimReader.CompanyId(User);
        var isValidToken = await refreshTokenService.ValidateTokenAsync(userId, request.RefreshToken);

        if (!isValidToken)
        {
            return BadRequest("Invalid client request");
        }

        var user = await userService.GetUserForRefreshAsync(userId);

        if (user is null)
        {
            return BadRequest(TextConstants.PermissionDenied);
        }

        var response = await PopulateTokenAsync(user, companyId);

        await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken);

        return response;
    }


    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(UserResponse user, Guid companyId)
    {
        var roles = await userRoleService.GetRolesByUserAsync(user.Id);

        if (roles.Length == 0)
        {
            logger.LogInformation("User without role - {email}", [user.Email]);
            return BadRequest(TextConstants.UserWithoutRoles);
        }

        var modules = await subscriptionCreditService.GetActiveModulesTypesAsync(companyId);
        var token = tokenService.GenerateToken(user, roles, companyId, modules);
        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        logger.LogInformation("User logged in - {email}", [user.Email]);

        return Ok(new TokenResponse
        {
            Token = token,
            RefreshToken = refreshToken
        });
    }
}