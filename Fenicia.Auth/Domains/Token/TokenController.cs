namespace Fenicia.Auth.Domains.Token;

using System.Net.Mime;

using Common;
using Common.Database.Requests;

using Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Fenicia.Auth.Domains.Company;
using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.SubscriptionCredit;
using Fenicia.Auth.Domains.User;
using Fenicia.Auth.Domains.UserRole;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> _logger;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IUserService _userService;
    private readonly IUserRoleService _userRoleService;
    private readonly ICompanyService _companyService;
    private readonly ISubscriptionCreditService _subscriptionCreditService;

    public TokenController(ILogger<TokenController> logger, ITokenService tokenService, IRefreshTokenService refreshTokenService, IUserService userService, IUserRoleService userRoleService, ICompanyService companyService, ISubscriptionCreditService subscriptionCreditService)
    {
        _logger = logger;
        _tokenService = tokenService;
        _refreshTokenService = refreshTokenService;
        _userService = userService;
        _userRoleService = userRoleService;
        _companyService = companyService;
        _subscriptionCreditService = subscriptionCreditService;
    }

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
            _logger.LogInformation("Starting token generation for user {Email}", request.Email);

            var company = await _companyService.GetByCnpjAsync(request.Cnpj, cancellationToken);

            if (company.Data is null)
            {
                _logger.LogWarning("Company not found for CNPJ {Cnpj}", request.Cnpj);
                return StatusCode((int)company.Status, company.Message);
            }

            var userResponse = await _userService.GetForLoginAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                _logger.LogWarning("User not found or invalid credentials for {Email}", request.Email);
                return StatusCode((int)userResponse.Status, userResponse.Message);
            }

            var response = await PopulateTokenAsync(userResponse.Data, company.Data.Id, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error generating token for user {Email}", request.Email);
            throw;
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Starting token refresh for user {UserID}", request.UserId);

            var isValidToken = await _refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);

            if (!isValidToken.Data)
            {
                _logger.LogWarning("Invalid refresh token for user {UserID}", request.UserId);
                return BadRequest("Invalid client request");
            }

            await _refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

            var userResponse = await _userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

            if (userResponse.Data is null)
            {
                _logger.LogWarning("User not found for refresh token {UserID}", request.UserId);
                return BadRequest(TextConstants.PermissionDenied);
            }

            var response = await PopulateTokenAsync(userResponse.Data, request.CompanyId, cancellationToken);

            return response;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error refreshing token for user {UserID}", request.UserId);
            throw;
        }
    }

    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(UserResponse user, Guid companyId, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Populating token for user {Email}", user.Email);

            var roles = await _userRoleService.GetRolesByUserAsync(user.Id, cancellationToken);

            if (roles.Data is null)
            {
                _logger.LogWarning("Unable to retrieve roles for user {Email}", user.Email);
                return StatusCode((int)roles.Status, roles.Message);
            }

            if (roles.Data.Length == 0)
            {
                _logger.LogWarning("User {Email} has no assigned roles", user.Email);
                return BadRequest(TextConstants.UserWithoutRoles);
            }

            var modules = await _subscriptionCreditService.GetActiveModulesTypesAsync(companyId, cancellationToken);

            if (modules.Data is null)
            {
                _logger.LogWarning("Unable to retrieve active modules for company {CompanyID}", companyId);
                return StatusCode((int)modules.Status, modules.Message);
            }

            var token = _tokenService.GenerateToken(user, roles.Data, companyId, modules.Data);

            if (token.Data is null)
            {
                _logger.LogWarning("Failed to generate token for user {Email}", user.Email);
                return StatusCode((int)token.Status, token.Message);
            }

            var refreshToken = await _refreshTokenService.GenerateRefreshTokenAsync(user.Id, cancellationToken);

            if (refreshToken.Data is null)
            {
                _logger.LogWarning("Failed to generate refresh token for user {Email}", user.Email);
                return StatusCode((int)refreshToken.Status, refreshToken.Message);
            }

            _logger.LogInformation("Successfully generated tokens for user {Email}", user.Email);

            return Ok(new TokenResponse
            {
                AccessToken = token.Data,
                RefreshToken = refreshToken.Data,
                User = new UserResponse()
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error populating token for user {Email}", user.Email);
            throw;
        }
    }
}
