using System.Net.Mime;
using Fenicia.Auth.Domains.Token.Interfaces;
using Fenicia.Common.DTOs.Auth.Token;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

/// <summary>
/// Controller for managing authentication token operations.
/// </summary>
[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TokenController(ITokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Generates a JWT token for the user (login).
    /// </summary>
    /// <param name="request">Request with email and password</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>JWT token and refresh token</returns>
    /// <response code="201">Token generated successfully</response>
    /// <response code="400">Invalid email or password, empty password, or too many attempts</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(
        TokenRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userResponse = await tokenService.GenerateAsync(request, cancellationToken);

            return Ok(userResponse);
        }
        catch (PermissionDeniedException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
        }
        catch (BadRequestException ex)
        {
            return BadRequest(
                new ProblemDetails
                {
                    Title = ex.Message,
                    Status = StatusCodes.Status400BadRequest
                });
        }
    }
}
