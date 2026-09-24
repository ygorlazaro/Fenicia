using System.Net.Mime;
using Fenicia.Common.API;
using Fenicia.Common.Data;
using Fenicia.Common.DTOs.SocialNetwork.Like;
using Fenicia.Module.SocialNetwork.Domains.Like.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

/// <summary>
///     Gerencia operações de curtidas em feeds.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class LikeController(
    ILikeService likeService,
    ICompanyContext companyContext,
    IProfileService profileService) : ControllerBase
{
    /// <summary>
    ///     Curte um feed.
    /// </summary>
    /// <param name="command">Dados da curtida</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Curtida criada</returns>
    /// <response code="201">Curtida criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddLikeResponse))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddLikeResponse>> PostAsync(
            [FromBody] LikeCommand command,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var result = await likeService.LikeAsync(
            command,
            companyContext.CompanyId,
            profileId,
            cancellationToken);

        return new CreatedResult(string.Empty, result);
    }

    /// <summary>
    ///     Remove a curtida de um feed.
    /// </summary>
    /// <param name="feedId">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Descurtido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnlikeAsync(
            [FromRoute] Guid feedId,
            CancellationToken cancellationToken = default)
    {

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        await likeService.UnlikeAsync(new UnlikeCommand(feedId), profileId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Obtém as curtidas de um feed.
    /// </summary>
    /// <param name="feedId">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="query">Termo de busca</param>
    /// <param name="sort">Ordenação</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de curtidas</returns>
    /// <response code="200">Lista de curtidas</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("feed/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetLikesResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetLikesResponse>>> GetLikesByFeedAsync(
            [FromRoute] Guid feedId,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            [FromQuery] string? query = null,
            [FromQuery] string? sort = null,
            CancellationToken cancellationToken = default)
    {

        var result = await likeService.GetLikesByFeedAsync(
            new GetLikesByFeedQuery(page, perPage, feedId, query, sort),
            cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Verifica se um perfil curtiu um feed.
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="feedId">ID do feed</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se curtiu, false caso contrário</returns>
    /// <response code="200">Resultado da verificação</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("isfollowed/{profileId:guid}/{feedId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(bool))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsLikedAsync(
            [FromRoute] Guid profileId,
            [FromRoute] Guid feedId,
            CancellationToken cancellationToken = default)
    {

        var result = await likeService.IsLikedAsync(new IsLikedQuery(), profileId, feedId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Obtém os feeds curtidos por um perfil.
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista de feeds curtidos</returns>
    /// <response code="200">Lista de feeds curtidos</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("profile/{profileId:guid}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(List<GetLikedFeedsResponse>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<List<GetLikedFeedsResponse>>> GetLikedFeedsByProfileAsync(
            [FromRoute] Guid profileId,
            [FromQuery] int page = 1,
            [FromQuery] int perPage = 10,
            CancellationToken cancellationToken = default)
    {

        var result = await likeService.GetLikedFeedsByProfileAsync(
            new GetLikedFeedsByProfileQuery(page, perPage, profileId),
            cancellationToken);

        return Ok(result);
    }

    private async Task<Guid> GetCurrentProfileIdAsync(CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var profile = await profileService.GetByUserIdAsync(userId, cancellationToken)
                      ?? throw new InvalidOperationException("Perfil social não encontrado para o usuário atual.");
        return profile.Id;
    }
}
