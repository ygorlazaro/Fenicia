using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken.GenerateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.InvalidateRefreshToken;
using Fenicia.Auth.Domains.RefreshToken.ValidateToken;
using Fenicia.Auth.Domains.Token.GenerateToken;
using Fenicia.Auth.Domains.Token.GenerateTokenString;
using Fenicia.Auth.Domains.User.GetUserForRefresh;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(
    GenerateRefreshTokenHandler generateRefreshTokenHandler,
    GenerateTokenStringHandler generateTokenStringHandler) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(
        [FromServices] GenerateTokenHandler handler,
        GenerateTokenQuery request,
        WideEventContext wide,
        CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await handler.Handle(request, ct);

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
        ValidateTokenQuery request,
        [FromServices] ValidateTokenHandler validateTokenHandler,
        [FromServices] InvalidateRefreshTokenHandler invalidateRefreshTokenHandler,
        [FromServices] GetUserForRefreshHandler getUserForRefreshHandler,
        WideEventContext wide,
        CancellationToken ct)
    {
        wide.UserId = request.UserId.ToString();

        var isValidToken = await validateTokenHandler.Handle(request);

        if (!isValidToken) return BadRequest("Invalid client request");

        await invalidateRefreshTokenHandler.Handler(request.RefreshToken, ct);

        var userResponse = await getUserForRefreshHandler.Handle(request.UserId, ct);

        return PopulateToken(new GenerateTokenResponse(userResponse.Id, userResponse.Name, userResponse.Email));
    }

    private ActionResult<TokenResponse> PopulateToken(GenerateTokenResponse user)
    {
        var token = generateTokenStringHandler.Handle(user);
        var refreshToken = generateRefreshTokenHandler.Handle(user.Id);

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