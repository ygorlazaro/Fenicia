using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Module.Basic.Domains.State.DTOs;
using Fenicia.Module.Basic.Domains.State.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Basic.Domains.State;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class StateController(IStateService stateService) : ControllerBase
{
    /// <summary>
    ///     Obtém a lista de estados.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="query">Filtros avançados. Example: <c>uf[=]SP</c></param>
    /// <param name="sort">Ordenação. Example: <c>uf</c></param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de estados</returns>
    /// <response code="200">Lista de estados retornada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os estados</exception>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetAllStateResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetAllStateResponse>>> GetAllAsync(
        WideEventContext wide,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        try
        {
            wide.UserId = ClaimReader.UserId(User).ToString();

            var states = await stateService.GetAllAsync(new GetAllStateQuery(1, 10, query, sort), cancellationToken);

            return Ok(states);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
    }
}