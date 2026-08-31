using System.Net.Mime;

using Fenicia.Auth.Domains.Token.DTOs;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Token;

[Authorize]
[Route("[controller]")]
[ApiController]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TokenController(TokenService tokenService) : ControllerBase
{
    /// <summary>
    /// Gera um token JWT para o usuário (login).
    /// </summary>
    /// <param name="request">Query com e-mail e senha</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Token JWT e refresh token</returns>
    /// <response code="201">Token gerado com sucesso</response>
    /// <response code="400">E-mail ou senha inválidos, senha vazia ou muitas tentativas</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TokenResponse>> PostAsync(GenerateTokenQuery request, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = request.Email;

            var userResponse = await tokenService.GenerateAsync(request, cancellationToken);

            return await PopulateTokenAsync(userResponse, cancellationToken);
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

    private async Task<ActionResult<TokenResponse>> PopulateTokenAsync(GenerateTokenResponse user, CancellationToken cancellationToken = default)
    {
        var token = tokenService.GenerateString(user);
        var response = token.MapToTokenResponse(string.Empty, user);

        return Created(string.Empty, response);
    }
}
