using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.Project.Team;
using Fenicia.Module.Projects.Domains.Team.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.Projects.Domains.Team;

/// <summary>
///     Gerencia operações de equipes.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class TeamController(
    ITeamService teamService,
    ICompanyContext companyContext) : ControllerBase
{
    /// <summary>
    ///     Obtém todas as equipes de um projeto com paginação.
    /// </summary>
    /// <param name="projectId">ID do projeto</param>
    /// <param name="query">Parâmetros de paginação</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de equipes</returns>
    /// <response code="200">Lista de equipes</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("project/{projectId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<TeamResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<TeamResponse>>> GetByProjectAsync(
        [FromRoute] Guid projectId,
        [FromQuery] GetAllTeamQuery query,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var teams = await teamService.GetAllByProjectAsync(projectId, query, cancellationToken);

        return Ok(teams);
    }

    /// <summary>
    ///     Obtém uma equipe pelo ID.
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Equipe encontrada</returns>
    /// <response code="200">Equipe encontrada</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Equipe não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<TeamResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var team = await teamService.GetByIdAsync(id, cancellationToken);

        return team is null ? NotFound() : Ok(team);
    }

    /// <summary>
    ///     Cria uma nova equipe.
    /// </summary>
    /// <param name="command">Dados da equipe</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Equipe criada</returns>
    /// <response code="201">Equipe criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(TeamResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TeamResponse>> PostAsync(
        [FromBody] TeamRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var team = await teamService.AddAsync(command, companyContext.CompanyId, cancellationToken);

        return new CreatedResult(string.Empty, team);
    }

    /// <summary>
    ///     Atualiza uma equipe existente.
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <param name="command">Dados atualizados da equipe</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Equipe atualizada</returns>
    /// <response code="200">Equipe atualizada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="404">Equipe não encontrada</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(TeamResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<TeamResponse>> PatchAsync(
        [FromRoute] Guid id,
        [FromBody] TeamRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        command.Id = id;
        var team = await teamService.UpdateAsync(command, companyContext.CompanyId, cancellationToken);

        return team is null ? NotFound() : Ok(team);
    }

    /// <summary>
    ///     Remove uma equipe.
    /// </summary>
    /// <param name="id">ID da equipe</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Equipe removida com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> DeleteAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await teamService.DeleteAsync(new DeleteTeamCommand(id), cancellationToken);

        return NoContent();
    }
}