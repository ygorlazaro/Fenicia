using System.Net.Mime;

using Fenicia.Auth.Domains.Company.Logic;
using Fenicia.Auth.Domains.RefreshToken.Data;
using Fenicia.Auth.Domains.RefreshToken.Logic;
using Fenicia.Auth.Domains.SubscriptionCredit.Logic;
using Fenicia.Auth.Domains.Token.Logic;
using Fenicia.Auth.Domains.User.Data;
using Fenicia.Auth.Domains.User.Logic;
using Fenicia.Auth.Domains.UserRole.Logic;
using Fenicia.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

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
    ISubscriptionCreditService subscriptionCreditService
) : ControllerBase
{
    /// <summary>
    /// Generates an authentication token for the user
    /// </summary>
    /// <param name="request">The token request containing credentials</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the authentication token</response>
    /// <response code="400">If the user has no roles assigned</response>
    /// <response code="404">If the company is not found</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(TokenRequest request, CancellationToken cancellationToken)
    {
        var company = await companyService.GetByCnpjAsync(request.Cnpj, cancellationToken);

        if (company.Data is null)
        {
            logger.LogInformation("Invalid login - {email}", request.Email);
            return StatusCode((int)company.Status, company.Message);
        }

        var userResponse = await userService.GetForLoginAsync(request, cancellationToken);

        if (userResponse.Data is null)
        {
            return StatusCode((int)userResponse.Status, userResponse.Message);
        }

        var response = await PopulateTokenAsync(userResponse.Data, company.Data.Id, cancellationToken);

        return response;
    }

    /// <summary>
    /// Gemerate a new authentication token for the user
    /// </summary>
    /// <param name="request">The refresh token request containing the refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the authentication token</response>
    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Refreshing token");

        var isValidToken = await refreshTokenService.ValidateTokenAsync(
            request.UserId,
            request.RefreshToken,
            cancellationToken
        );

        if (!isValidToken.Data)
        {
            return BadRequest("Invalid client request");
        }

        await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var userResponse = await userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

        if (userResponse.Data is null)
        {
            return BadRequest(TextConstants.PermissionDenied);
        }

        var response = await PopulateTokenAsync(userResponse.Data, request.CompanyId, cancellationToken);


        return response;
    }

    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(UserResponse user,
        Guid companyId, CancellationToken cancellationToken)
    {
        var roles = await userRoleService.GetRolesByUserAsync(user.Id, cancellationToken);

        if (roles.Data is null)
        {
            return StatusCode((int)roles.Status, roles.Message);
        }

        if (roles.Data.Length == 0)
        {
            logger.LogInformation("User without role - {email}", user.Email);
            return BadRequest(TextConstants.UserWithoutRoles);
        }

        var modules = await subscriptionCreditService.GetActiveModulesTypesAsync(companyId, cancellationToken);

        if (modules.Data is null)
        {
            return StatusCode((int)modules.Status, modules.Message);
        }

        var token = tokenService.GenerateToken(user, roles.Data, companyId, modules.Data);

        if (token.Data is null)
        {
            return StatusCode((int)token.Status, token.Message);
        }

        var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

        if (refreshToken.Data is null)
        {
            return StatusCode((int)refreshToken.Status, refreshToken.Message);
        }

        logger.LogInformation("User logged in - {email}", user.Email);

        return Ok(new TokenResponse { Token = token.Data, RefreshToken = refreshToken.Data });
    }
}
