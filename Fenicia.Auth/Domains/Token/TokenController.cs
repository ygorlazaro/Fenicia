using System.Net.Mime;

using Fenicia.Auth.Domains.RefreshToken;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Auth.Domains.User;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
public class TokenController(TokenService tokenService, RefreshTokenService refreshTokenService, UserService userService) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(GenerateTokenQuery request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await tokenService.GenerateAsync(request, ct);

            return await PopulateTokenAsync(userResponse, ct);
        }
        catch (PermissionDeniedException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
        catch (InvalidRequestException ex)
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
    [ProducesResponseType(typeof(TokenResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TokenResponse>> Refresh(ValidateTokenQuery request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = request.UserId.ToString();

            var isValidToken = await refreshTokenService.ValidateAsync(request.UserId, request.RefreshToken, ct);

            if (!isValidToken)
            {
                return BadRequest("Invalid client request");
            }

            await refreshTokenService.InvalidateAsync(request.RefreshToken, ct);

            var userResponse = await userService.GetForRefreshAsync(request.UserId, ct);

            return await PopulateTokenAsync(new GenerateTokenResponse(userResponse.Id, userResponse.Name, userResponse.Email), ct);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(GenerateTokenResponse user, CancellationToken ct)
    {
        var token = tokenService.GenerateString(user);
        var refreshToken = refreshTokenService.Generate(user.Id);
        var response = token.MapToTokenResponse(refreshToken, user);

        return Created(string.Empty, response);
    }
}
