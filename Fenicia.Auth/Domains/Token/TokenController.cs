using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.Auth;
using Fenicia.Common.Data.Responses.Auth;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(
    ITokenService tokenService,
    IRefreshTokenService refreshTokenService,
    IUserService userService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(
        TokenRequest request,
        WideEventContext wide,
        CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await userService.GetForLoginAsync(request, ct);

            return PopulateToken(userResponse);
        }
        catch (PermissionDeniedException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(
        RefreshTokenRequest request,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.UserId.ToString();

        var isValidToken = await refreshTokenService.ValidateTokenAsync(request.UserId, request.RefreshToken, ct);

        if (!isValidToken)
        {
            return BadRequest("Invalid client request");
        }

        await refreshTokenService.InvalidateRefreshTokenAsync(request.RefreshToken, ct);

        var userResponse = await userService.GetUserForRefreshAsync(request.UserId, ct);

        return PopulateToken(userResponse);
    }

    private ActionResult<TokenResponse> PopulateToken(UserResponse user)
    {
        var token = tokenService.GenerateToken(user);
        var refreshToken = refreshTokenService.GenerateRefreshToken(user.Id);

        return Ok(new TokenResponse
        {
            AccessToken = token,
            RefreshToken = refreshToken,
            User = new UserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Name = user.Name
            }
        });
    }
}
