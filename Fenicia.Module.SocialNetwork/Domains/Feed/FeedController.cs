using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.SocialNetwork.Feed;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

/// <summary>
///     Gerencia operações de feed (posts).
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FeedController(
    IFeedService feedService,
    ICompanyContext companyContext,
    IProfileService profileService) : ControllerBase
{
    /// <summary>
    ///     Obtém todos os feeds com paginação.
    /// </summary>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Termo de busca</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de feeds</returns>
    /// <response code="200">Lista de feeds</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FeedResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<FeedResponse>>> GetAsync(
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.GetAllAsync(new GetAllFeedQuery(page, perPage, query, sort), cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Obtém um feed pelo ID.
    /// </summary>
    /// <param name="id">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Feed encontrado</returns>
    /// <response code="200">Feed encontrado</response>
    /// <response code="400">ID inválido</response>
    /// <response code="404">Feed não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<FeedResponse>> GetByIdAsync(
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.GetByIdAsync(new GetFeedByIdQuery(id), cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    ///     Obtém feeds de um perfil específico.
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de feeds do perfil</returns>
    /// <response code="200">Lista de feeds</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("profile/{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<FeedResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<FeedResponse>>> GetByProfileIdAsync(
        [FromRoute] Guid profileId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 20,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await feedService.GetByProfileIdAsync(
            new GetFeedsByProfileQuery(page, perPage, profileId),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Cria um novo feed.
    /// </summary>
    /// <param name="command">Dados do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Feed criado</returns>
    /// <response code="201">Feed criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(FeedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<FeedResponse>> PostAsync(
        [FromBody] FeedRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        command.ProfileId = profileId;
        var result = await feedService.AddAsync(
            command,
            companyContext.CompanyId,
            cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    ///     Atualiza um feed existente.
    /// </summary>
    /// <param name="command">Dados atualizados do feed</param>
    /// <param name="id">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Feed atualizado</returns>
    /// <response code="200">Feed atualizado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="404">Feed não encontrado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPatch("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(FeedResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<FeedResponse>> PatchAsync(
        [FromBody] FeedRequest command,
        [FromRoute] Guid id,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        command.Id = id;
        var result = await feedService.UpdateAsync(
            command,
            companyContext.CompanyId,
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    /// <summary>
    ///     Remove um feed.
    /// </summary>
    /// <param name="id">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Feed removido com sucesso</response>
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

        await feedService.DeleteAsync(new DeleteFeedCommand(id), cancellationToken);

        return NoContent();
    }

    private async Task<Guid> GetCurrentProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken)
                      ?? throw new InvalidOperationException("Perfil social não encontrado para o usuário atual.");
        return profile.Id;
    }
}
