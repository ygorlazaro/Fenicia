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
    /// <summary>
    /// Gera um novo refresh token para o usuário autenticado.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Token de atualização gerado</returns>
    /// <response code="200">Refresh token gerado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
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

    /// <summary>
    /// Valida um refresh token para o usuário autenticado.
    /// </summary>
    /// <param name="query">Query com UserId e RefreshToken</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da validação com dados do token</returns>
    /// <response code="200">Token válido</response>
    /// <response code="400">Refresh token inválido ou nulo</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
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

    /// <summary>
    /// Invalida um refresh token.
    /// </summary>
    /// <param name="command">Comando com o refresh token a ser invalidado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se invalidado com sucesso</returns>
    /// <response code="204">Refresh token invalidado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
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
