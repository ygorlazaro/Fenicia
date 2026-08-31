using System.Net.Mime;
using Fenicia.Auth.Domains.Company.DTOs;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Exceptions;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Auth.Domains.Company;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class CompanyController(CompanyService service) : ControllerBase
{
    /// <summary>
    /// Obtém as empresas associadas ao usuário autenticado com paginação.
    /// </summary>
    /// <param name="query">Parâmetros de paginação (página e quantidade por página)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Lista paginada de empresas do usuário</returns>
    /// <response code="200">Empresas encontradas para o usuário</response>
    /// <response code="400">Requisição inválida (ex: perPage menor ou igual a zero)</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não associado a empresas ativas</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetCompaniesByUserResponse>>>> GetByLoggedUser([FromQuery] PaginationQuery query, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            var result = await service.GetCompaniesByUserAsync(userId, query.Page, query.PerPage, ct);

            return Ok(result);
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid(ex.Message);
        }
        catch (InvalidRequestException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    /// <summary>
    /// Atualiza o nome de uma empresa existente.
    /// </summary>
    /// <param name="id">ID da empresa</param>
    /// <param name="request">Dados de atualização da empresa (nome)</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="ct">Token de cancelamento</param>
    /// <returns>Sem conteúdo (204) se atualizado com sucesso</returns>
    /// <response code="204">Empresa atualizada com sucesso</response>
    /// <response code="400">Requisição inválida</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não tem permissão de Admin para atualizar esta empresa</response>
    /// <response code="404">Empresa não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> PatchAsync([FromRoute] Guid id, [FromBody] UpdateCompanyRequest request, WideEventContext wide, CancellationToken ct)
    {
        try
        {
            var userId = ClaimReader.UserId(User);
            wide.UserId = userId.ToString();

            await service.UpdateAsync(id, userId, request.Name, ct);

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
        catch (PermissionDeniedException ex)
        {
            return Forbid(ex.Message);
        }
    }
}
