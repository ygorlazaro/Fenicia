using System.Net.Mime;
using Fenicia.Auth.Requests;
using Fenicia.Auth.Responses;
using Fenicia.Auth.Services.Interfaces;
using Fenicia.Common;
using Fenicia.Common.Api;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

        if (company.Data is null)
        {
            logger.LogInformation("Invalid login - {email}", [request.Email]);
            return StatusCode((int)company.StatusCode, company.Message);
        }

        var userResponse = await userService.GetForLoginAsync(request);

        if (userResponse.Data is null)
        {
            return StatusCode((int)userResponse.StatusCode, userResponse.Message);
        }

        var response = await PopulateTokenAsync(userResponse.Data, company.Data.Id);

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

        if (!isValidToken.Data)
        {
            return BadRequest("Invalid client request");
        }

        var userResponse = await userService.GetUserForRefreshAsync(userId);

        if (userResponse.Data is null)
        {
            return BadRequest(TextConstants.PermissionDenied);
        }

        var response = await PopulateTokenAsync(userResponse.Data, companyId);

        await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken);

        return response;
    }


    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(UserResponse user, Guid companyId)
    {
        var roles = await userRoleService.GetRolesByUserAsync(user.Id);

        if (roles.Data is null)
        {
            return StatusCode((int)roles.StatusCode, roles.Message);
        }

        if (roles.Data.Length == 0)
        {
            logger.LogInformation("User without role - {email}", [user.Email]);
            return BadRequest(TextConstants.UserWithoutRoles);
        }

        var modules = await subscriptionCreditService.GetActiveModulesTypesAsync(companyId);

        if (modules.Data is null)
        {
            return StatusCode((int)modules.StatusCode, modules.Message);
        }

        var token = tokenService.GenerateToken(user, roles.Data, companyId, modules.Data);

        if (token.Data is null)
        {
            return StatusCode((int)token.StatusCode, token.Message);
        }

        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id);

        if (refreshToken.Data is null)
        {
            return StatusCode((int)refreshToken.StatusCode, refreshToken.Message);
        }

        logger.LogInformation("User logged in - {email}", [user.Email]);

        return Ok(new TokenResponse
        {
            Token = token.Data,
            RefreshToken = refreshToken.Data
        });
    }
}