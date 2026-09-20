using System.Net.Mime;
using Fenicia.Auth.Domains.RefreshToken.Interfaces;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Auth.RefreshToken;
using Fenicia.Common.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.RefreshToken;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class RefreshTokenController(IRefreshTokenService refreshTokenService) : ControllerBase
{
    /// <summary>
    ///     Gera um novo refresh token para o usuário autenticado.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token de atualização gerado</returns>
    /// <response code="201">Refresh token gerado com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
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
    ///     Valida um refresh token pelo valor.
    /// </summary>
    /// <param name="token">Valor do refresh token</param>
    /// <returns>Resultado da validação com dados do token</returns>
    /// <response code="200">Token válido</response>
    /// <response code="400">Refresh token inválido ou nulo</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Token não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
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
        catch (InvalidRequestException ex)
        {
            return BadRequest(new { ex.Message });
        }
    }
}