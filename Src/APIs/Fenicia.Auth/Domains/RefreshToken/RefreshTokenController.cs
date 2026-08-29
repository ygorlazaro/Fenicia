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
    /// <response code="201">Refresh token gerado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GenerateRefreshTokenResponse))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<GenerateRefreshTokenResponse>> PostAsync(WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();
            var userId = ClaimReader.UserId(User);
            var token = await refreshTokenService.GenerateAsync(userId, ct);

            return new CreatedResult(string.Empty, new GenerateRefreshTokenResponse(token, DateTime.UtcNow.AddDays(7)));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }

    /// <summary>
    /// Valida um refresh token pelo valor.
    /// </summary>
    /// <param name="token">Valor do refresh token</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Resultado da validação com dados do token</returns>
    /// <response code="200">Token válido</response>
    /// <response code="400">Refresh token inválido ou nulo</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Token não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{token}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ValidateTokenResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ValidateTokenResponse>> GetAsync([FromRoute] string token, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var isValid = await refreshTokenService.ValidateAsync(userId, token, ct);
            var tokenData = await refreshTokenService.GetAsync(token, ct);

            if (tokenData is null)
            {
                return NotFound();
            }

            return Ok(new ValidateTokenResponse(token, tokenData.ExpirationDate, userId, isValid));
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
    /// <param name="token">Valor do refresh token</param>
    /// <param name="command">Comando com o refresh token a ser invalidado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se invalidado com sucesso</returns>
    /// <response code="204">Refresh token invalidado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Token não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{token}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<IActionResult> PatchAsync([FromRoute] string token, [FromBody] UpdateRefreshTokenCommand command, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            await refreshTokenService.UpdateAsync(token, command.IsActive, ct);

            return NoContent();
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (ItemNotExistsException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }
}
