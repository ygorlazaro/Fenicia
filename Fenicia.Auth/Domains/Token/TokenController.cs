using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken.Handlers;
using Fenicia.Auth.Domains.RefreshToken.Queries;
using Fenicia.Auth.Domains.Token.Handlers;
using Fenicia.Auth.Domains.Token.Queries;
using Fenicia.Auth.Domains.Token.Responses;
using Fenicia.Auth.Domains.User.Handlers;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(GenerateTokenHandler generateTokenHandler, GenerateRefreshTokenHandler generateRefreshTokenHandler, GenerateTokenStringHandler generateTokenStringHandler, ValidateTokenHandler validateTokenHandler, InvalidateRefreshTokenHandler invalidateRefreshTokenHandler, GetUserForRefreshHandler getUserForRefreshHandler) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(GenerateTokenQuery request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await generateTokenHandler.Handle(request, ct);

            return PopulateToken(userResponse);
        }
        catch (PermissionDeniedException ex)
        {
            return BadRequest(new ProblemDetails { Title = ex.Message, Status = StatusCodes.Status400BadRequest });
        }
    }

    [HttpPost]
    [AllowAnonymous]
    [Route("refresh")]
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(ValidateTokenQuery request, WideEventContext wide, CancellationToken ct)
    {
        wide.UserId = request.UserId.ToString();

        var isValidToken = await validateTokenHandler.Handle(request);

        if (!isValidToken)
        {
            return BadRequest("Invalid client request");
        }

        await invalidateRefreshTokenHandler.Handler(request.RefreshToken);

        var userResponse = await getUserForRefreshHandler.Handle(request.UserId, ct);

        return PopulateToken(new GenerateTokenResponse(userResponse.Id, userResponse.Name, userResponse.Email));
    }

    private ActionResult<TokenResponse> PopulateToken(GenerateTokenResponse user)
    {
        var token = generateTokenStringHandler.Handle(user);
        var refreshToken = generateRefreshTokenHandler.Handle(user.Id);
        var userResponse = new UserResponse(user.Id, user.Name, user.Email);
        var response = new TokenResponse(token, refreshToken, userResponse);

        return Created(string.Empty, response);
    }
}