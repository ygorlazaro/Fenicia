namespace Fenicia.Auth.Domains.Token;

using System.Net.Mime;

using Common;
using Common.Database.Requests;

using Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using RefreshToken;

using User;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController : ControllerBase
{
    private readonly ILogger<TokenController> logger;
    private readonly ITokenService tokenService;
    private readonly IRefreshTokenService refreshTokenService;
    private readonly IUserService userService;

    public TokenController(ILogger<TokenController> logger, ITokenService tokenService, IRefreshTokenService refreshTokenService, IUserService userService)
    {
        this.logger = logger;
        this.tokenService = tokenService;
        this.refreshTokenService = refreshTokenService;
        this.userService = userService;
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
            this.logger.LogInformation("Starting token generation for user {Email}", request.Email);

            var userResponse = await this.userService.GetForLoginAsync(request, cancellationToken);

            if (userResponse.Data is null)
            {
                this.logger.LogWarning("User not found or invalid credentials for {Email}", request.Email);
                return this.StatusCode((int)userResponse.Status, userResponse.Message);
            }

            return this.PopulateToken(userResponse.Data);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error generating token for user {Email}", request.Email);
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
            this.logger.LogInformation("Starting token refresh for user {UserID}", request.UserId);

            var isValidToken = await this.refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);

            if (!isValidToken.Data)
            {
                this.logger.LogWarning("Invalid refresh token for user {UserID}", request.UserId);
                return this.BadRequest("Invalid client request");
            }

            await this.refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

            var userResponse = await this.userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

            if (userResponse.Data is null)
            {
                this.logger.LogWarning("User not found for refresh token {UserID}", request.UserId);
                return this.BadRequest(TextConstants.PermissionDeniedMessage);
            }

            return this.PopulateToken(userResponse.Data);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error refreshing token for user {UserID}", request.UserId);
            throw;
        }
    }

    private ActionResult<TokenResponse> PopulateToken(UserResponse user)
    {
        try
        {
            this.logger.LogInformation("Populating token for user {UserId}", user.Id);

            var token = this.tokenService.GenerateToken(user);

            if (token.Data is null)
            {
                this.logger.LogWarning("Failed to generate token for user {Email}", user.Email);
                return this.StatusCode((int)token.Status, token.Message);
            }

            var refreshToken = this.refreshTokenService.GenerateRefreshToken(user.Id);

            if (refreshToken.Data is null)
            {
                this.logger.LogWarning("Failed to generate refresh token for user {Email}", user.Email);
                return this.StatusCode((int)refreshToken.Status, refreshToken.Message);
            }

            this.logger.LogInformation("Successfully generated tokens for user {Email}", user.Email);

            return this.Ok(new TokenResponse
            {
                AccessToken = token.Data,
                RefreshToken = refreshToken.Data,
                User = new UserResponse
                       {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name
                }
            });
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error populating token for user {Email}", user.Email);
            throw;
        }
    }
}
