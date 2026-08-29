using System.Net.Mime;
using Fenicia.Auth.Domains.RefreshToken.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.RefreshToken;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RefreshTokenController(RefreshTokenService refreshTokenService) : ControllerBase
{
    [HttpPost("generate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GenerateRefreshTokenCommand))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<string>> GenerateAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var userId = ClaimReader.UserId(User);
            var token = await refreshTokenService.GenerateAsync(userId, ct);
            return Ok(token);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    [HttpPost("validate")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidateTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<ValidateTokenResponse>> ValidateAsync([FromBody] ValidateTokenQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var userId = ClaimReader.UserId(User);
            var isValid = await refreshTokenService.ValidateAsync(userId, query.RefreshToken, ct);

            return Ok(new ValidateTokenResponse(query.RefreshToken, DateTime.UtcNow.AddDays(7), userId, isValid));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    [HttpPost("invalidate")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult> InvalidateAsync([FromBody] InvalidateRefreshTokenCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            await refreshTokenService.InvalidateAsync(command.RefreshToken, ct);
            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
