namespace Fenicia.Auth.Domains.Token;

using System.Net.Mime;

using Common;

using Company.Logic;

using Data;

using Logic;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RefreshToken.Data;
using RefreshToken.Logic;

using SubscriptionCredit.Logic;

using User.Data;
using User.Logic;

using UserRole.Logic;

/// <summary>
///     Controller responsible for handling token-related operations
/// </summary>
[Authorize]
[Route(template: "[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(ILogger<TokenController> logger, ITokenService tokenService, IRefreshTokenService refreshTokenService, IUserService userService, IUserRoleService userRoleService, ICompanyService companyService, ISubscriptionCreditService subscriptionCreditService) : ControllerBase
{
    /// <summary>
    ///     Generates an authentication token for the user
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
        try
        {
            logger.LogInformation(message: "Starting token generation for user {Email}", request.Email);

            var company = await companyService.GetByCnpjAsync(request.Cnpj, cancellationToken);

            if (company.Data is null)
            {
                logger.LogWarning(message: "Company not found for CNPJ {Cnpj}", request.Cnpj);
                return StatusCode((int)company.Status, company.Message);
            }

            var userResponse = await userService.GetForLoginAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                logger.LogWarning(message: "User not found or invalid credentials for {Email}", request.Email);
                return StatusCode((int)userResponse.Status, userResponse.Message);
            }

            var response = await PopulateTokenAsync(userResponse.Data, company.Data.Id, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error generating token for user {Email}", request.Email);
            throw;
        }
    }

    /// <summary>
    ///     Gemerate a new authentication token for the user
    /// </summary>
    /// <param name="request">The refresh token request containing the refresh token</param>
    /// <param name="cancellationToken"></param>
    /// <response code="200">Returns the authentication token</response>
    [HttpPost]
    [AllowAnonymous]
    [Route(template: "refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Starting token refresh for user {UserId}", request.UserId);

            var isValidToken = await refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);

            if (!isValidToken.Data)
            {
                logger.LogWarning(message: "Invalid refresh token for user {UserId}", request.UserId);
                return BadRequest(error: "Invalid client request");
            }

            await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

            var userResponse = await userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

            if (userResponse.Data is null)
            {
                logger.LogWarning(message: "User not found for refresh token {UserId}", request.UserId);
                return BadRequest(TextConstants.PermissionDenied);
            }

            var response = await PopulateTokenAsync(userResponse.Data, request.CompanyId, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error refreshing token for user {UserId}", request.UserId);
            throw;
        }
    }

    /// <summary>
    ///     Populates and generates a new token response with user information
    /// </summary>
    /// <param name="user">User information</param>
    /// <param name="companyId">Company identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Token response containing authentication and refresh tokens</returns>
    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(UserResponse user, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation(message: "Populating token for user {Email}", user.Email);

            var roles = await userRoleService.GetRolesByUserAsync(user.Id, cancellationToken);

            if (roles.Data is null)
            {
                logger.LogWarning(message: "Unable to retrieve roles for user {Email}", user.Email);
                return StatusCode((int)roles.Status, roles.Message);
            }

            if (roles.Data.Length == 0)
            {
                logger.LogWarning(message: "User {Email} has no assigned roles", user.Email);
                return BadRequest(TextConstants.UserWithoutRoles);
            }

            var modules = await subscriptionCreditService.GetActiveModulesTypesAsync(companyId, cancellationToken);

            if (modules.Data is null)
            {
                logger.LogWarning(message: "Unable to retrieve active modules for company {CompanyId}", companyId);
                return StatusCode((int)modules.Status, modules.Message);
            }

            var token = tokenService.GenerateToken(user, roles.Data, companyId, modules.Data);

            if (token.Data is null)
            {
                logger.LogWarning(message: "Failed to generate token for user {Email}", user.Email);
                return StatusCode((int)token.Status, token.Message);
            }

            var refreshToken = await refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

            if (refreshToken.Data is null)
            {
                logger.LogWarning(message: "Failed to generate refresh token for user {Email}", user.Email);
                return StatusCode((int)refreshToken.Status, refreshToken.Message);
            }

            logger.LogInformation(message: "Successfully generated tokens for user {Email}", user.Email);

            return Ok(new TokenResponse { Token = token.Data, RefreshToken = refreshToken.Data });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error populating token for user {Email}", user.Email);
            throw;
        }
    }
}
