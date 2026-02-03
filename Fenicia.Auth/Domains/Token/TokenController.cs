using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.User;
using Fenicia.Common;
using Fenicia.Common.Api;
using Fenicia.Common.Database.Requests;
using Fenicia.Common.Database.Responses;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(ITokenService tokenService, IRefreshTokenService refreshTokenService, IUserService userService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(TokenRequest request, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Login";
        wide.UserId = request.Email;

        var userResponse = await userService.GetForLoginAsync(request, cancellationToken);

        if (userResponse.Data is not null)
        {
            return PopulateToken(userResponse.Data);
        }

        return StatusCode((int)userResponse.Status, userResponse.Message);
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(RefreshTokenRequest request, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.Operation = "Refresh Token";
        wide.UserId = request.UserId.ToString();

        var isValidToken = await refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, cancellationToken);

        if (!isValidToken.Data)
        {
            return BadRequest("Invalid client request");
        }

        await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, cancellationToken);

        var userResponse = await userService.GetUserForRefreshAsync(request.UserId, cancellationToken);

        if (userResponse.Data is not null)
        {
            return PopulateToken(userResponse.Data);
        }

        return BadRequest(TextConstants.PermissionDeniedMessage);
    }

    private ActionResult<TokenResponse> PopulateToken(UserResponse user)
    {
        var token = tokenService.GenerateToken(user);

        if (token.Data is null)
        {
            return StatusCode((int)token.Status, token.Message);
        }

        var refreshToken = refreshTokenService.GenerateRefreshToken(user.Id);

        if (refreshToken.Data is null)
        {
            return StatusCode((int)refreshToken.Status, refreshToken.Message);
        }

        return Ok(new TokenResponse
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
}
