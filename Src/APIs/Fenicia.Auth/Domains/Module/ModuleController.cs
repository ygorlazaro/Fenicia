using System.Net.Mime;
using Fenicia.Auth.Domains.Module.DTOs;
using Fenicia.Common;
using Fenicia.Common.API;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Module;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class ModuleController(ModuleService service) : ControllerBase
{
    /// <summary>
    /// Obtém todos os módulos ativos com paginação (endpoint anônimo).
    /// </summary>
    /// <param name="query">Parâmetros de paginação (página e quantidade por página)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de módulos</returns>
    /// <response code="200">Lista de módulos retornada com sucesso</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(typeof(Pagination<List<GetModuleResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetModuleResponse>>> GetAllModulesAsync([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken cancellationToken)
    {
        wide.UserId = "Guest";

        var modules = await service.GetAllModulesAsync(query.Page, query.PerPage, cancellationToken);

        return Ok(modules);
    }
}
