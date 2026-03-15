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
    /// <summary>
    ///     Generates an authentication token for the user.
    /// </summary>
    /// <param name="request">The token request containing email and password.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Authentication token response.</returns>
    /// <response code="201">Token generated successfully.</response>
    /// <response code="400">Invalid request or invalid credentials.</response>
    /// <exception cref="PermissionDeniedException">Invalid username or password.</exception>
    /// <exception cref="InvalidRequestException">Invalid request - email or password is empty.</exception>
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

            var userResponse = await generateTokenHandler.Handle(request, ct);

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
        catch (InvalidRequestException ex)
        {
            return BadRequest(new ProblemDetails
            {
                Title = ex.Message,
                Status = StatusCodes.Status400BadRequest
            });
        }
    }

    /// <summary>
    ///     Refreshes an authentication token using a valid refresh token.
    /// </summary>
    /// <param name="request">The validation query containing user ID and refresh token.</param>
    /// <param name="wide">Wide event context for request tracking.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>New authentication token response.</returns>
    /// <response code="201">Token refreshed successfully.</response>
    /// <response code="400">Invalid request or refresh token.</response>
    /// <exception cref="InvalidRequestException">Refresh token is null or whitespace.</exception>
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

            var isValidToken = await validateTokenHandler.Handle(request);

            if (!isValidToken)
            {
                return BadRequest("Invalid client request");
            }

            await invalidateRefreshTokenHandler.Handler(request.RefreshToken);

            var userResponse = await getUserForRefreshHandler.Handle(request.UserId, ct);

            return PopulateToken(new GenerateTokenResponse(userResponse.Id, userResponse.Name, userResponse.Email));
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

    private ActionResult<TokenResponse> PopulateToken(GenerateTokenResponse user)
    {
        var token = generateTokenStringHandler.Handle(user);
        var refreshToken = generateRefreshTokenHandler.Handle(user.Id);
        var userResponse = new UserResponse(user.Id, user.Name, user.Email);
        var response = new TokenResponse(token, refreshToken, userResponse);

        return Created(string.Empty, response);
    }
}
