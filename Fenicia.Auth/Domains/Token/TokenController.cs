using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(ILogger<TokenController> logger, ITokenService tokenService, IRefreshTokenService refreshTokenService, IUserService userService) : ControllerBase
{
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
            logger.LogInformation("Starting token generation for user {Email}", request.Email);

            var userResponse = await userService.GetForLoginAsync(request, cancellationToken);

            if (userResponse.Data is not null)
            {
                return this.PopulateToken(userResponse.Data);
            }

            logger.LogWarning("User not found or invalid credentials for {Email}", request.Email);

            return this.StatusCode((int)userResponse.Status, userResponse.Message);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error generating token for user {Email}", request.Email);

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
            logger.LogInformation("Starting token refresh for user {UserID}", request.UserId);

            var isValidToken = await refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);

            if (!isValidToken.Data)
            {
                logger.LogWarning("Invalid refresh token for user {UserID}", request.UserId);
                return this.BadRequest("Invalid client request");
            }

            await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

            var userResponse = await userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

            if (userResponse.Data is not null)
            {
                return this.PopulateToken(userResponse.Data);
            }

            logger.LogWarning("User not found for refresh token {UserID}", request.UserId);

            return this.BadRequest(TextConstants.PermissionDeniedMessage);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error refreshing token for user {UserID}", request.UserId);

            throw;
        }
    }

    private ActionResult<TokenResponse> PopulateToken(UserResponse user)
    {
        try
        {
            logger.LogInformation("Populating token for user {UserId}", user.Id);

            var token = tokenService.GenerateToken(user);

            if (token.Data is null)
            {
                logger.LogWarning("Failed to generate token for user {Email}", user.Email);
                return this.StatusCode((int)token.Status, token.Message);
            }

            var refreshToken = refreshTokenService.GenerateRefreshToken(user.Id);

            if (refreshToken.Data is null)
            {
                logger.LogWarning("Failed to generate refresh token for user {Email}", user.Email);
                return this.StatusCode((int)refreshToken.Status, refreshToken.Message);
            }

            logger.LogInformation("Successfully generated tokens for user {Email}", user.Email);

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
            logger.LogError(ex, "Error populating token for user {Email}", user.Email);
            throw;
        }
    }
}
