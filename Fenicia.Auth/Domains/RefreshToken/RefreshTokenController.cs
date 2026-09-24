using System.Net.Mime;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.RefreshToken;

/// <summary>
/// Controller responsible for handling refresh token operations, including generating new refresh tokens and validating existing ones. It provides endpoints for authenticated users to manage their refresh tokens securely.
/// </summary>
/// <param name="refreshTokenService"></param>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RefreshTokenController(IRefreshTokenService refreshTokenService) : ControllerBase
{
    /// <summary>
    /// Generates a new refresh token for the authenticated user.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Generated refresh token</returns>
    /// <response code="201">Refresh token generated successfully</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="500">Internal server error</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(RefreshTokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<RefreshTokenResponse>> PostAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            var response = await refreshTokenService.GenerateAsync(userId, cancellationToken);

            return new CreatedResult(string.Empty, response);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Validates a refresh token by its value.
    /// </summary>
    /// <param name="token">Refresh token value</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Validation result with token data</returns>
    /// <response code="200">Token is valid</response>
    /// <response code="400">Refresh token is invalid or null</response>
    /// <response code="401">User not authenticated</response>
    /// <response code="404">Token not found</response>
    /// <response code="500">Internal server error</response>
    [HttpGet("{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(RefreshTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<RefreshTokenResponse>> GetAsync(
        [FromRoute] string token,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var userId = ClaimReader.UserId(User);

            var isValid = await refreshTokenService.ValidateAsync(userId, token, cancellationToken);
            var tokenData = await refreshTokenService.GetAsync(token, cancellationToken);

            if (tokenData is null)
            {
                return NotFound();
            }

            tokenData.IsActive = isValid;
            return Ok(tokenData);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (BadRequestException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}
