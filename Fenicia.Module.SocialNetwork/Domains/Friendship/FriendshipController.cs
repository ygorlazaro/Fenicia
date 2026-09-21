using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.SocialNetwork.Friendship;
using Fenicia.Module.SocialNetwork.Domains.Friendship.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

/// <summary>
///     Gerencia operações de amizade/seguidores entre perfis.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FriendshipController(IFriendshipService friendshipService, IProfileService profileService) : ControllerBase
{
/// <summary>
///     Segue um perfil.
/// </summary>
/// <param name="command">Dados do seguimento</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Seguimento criado ou reativado</returns>
/// <response code="201">Seguimento criado com sucesso</response>
/// <response code="400">Dados inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpPost("follow")]
[ProducesResponseType(typeof(AddFriendshipResponse), StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
[Consumes(MediaTypeNames.Application.Json)]
public async Task<ActionResult<AddFriendshipResponse>> FollowAsync(
    [FromBody] FollowCommand command,
    WideEventContext wide,
    CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var friendship = await friendshipService.FollowAsync(command, profileId, cancellationToken);

        return new CreatedResult(string.Empty, friendship);
    }

/// <summary>
///     Deixa de seguir um perfil.
/// </summary>
/// <param name="targetProfileId">ID do perfil alvo</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Sem conteúdo</returns>
/// <response code="204">Deixou de seguir com sucesso</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpDelete("unfollow/{targetProfileId:guid}")]
[ProducesResponseType(StatusCodes.Status204NoContent)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult> UnfollowAsync(
        [FromRoute] Guid targetProfileId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        await friendshipService.UnfollowAsync(
            new UnfollowCommand(targetProfileId),
            profileId,
            cancellationToken);

        return NoContent();
    }

/// <summary>
///     Obtém a lista de seguidores de um perfil.
/// </summary>
/// <param name="targetProfileId">ID do perfil alvo</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="page">Página</param>
/// <param name="perPage">Itens por página</param>
/// <param name="query">Termo de busca</param>
/// <param name="sort">Ordenação</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Lista paginada de seguidores</returns>
/// <response code="200">Lista de seguidores</response>
/// <response code="400">Parâmetros inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpGet("followers/{targetProfileId:guid}")]
[ProducesResponseType(typeof(Pagination<List<GetFollowersResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<Pagination<List<GetFollowersResponse>>>> GetFollowersAsync(
        [FromRoute] Guid targetProfileId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.GetFollowersAsync(
            new GetFollowersQuery(page, perPage, query, sort),
            targetProfileId,
            cancellationToken);

        return Ok(result);
    }

/// <summary>
///     Obtém a lista de perfis que um perfil segue.
/// </summary>
/// <param name="profileId">ID do perfil</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="page">Página</param>
/// <param name="perPage">Itens por página</param>
/// <param name="query">Termo de busca</param>
/// <param name="sort">Ordenação</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>Lista paginada de seguidos</returns>
/// <response code="200">Lista de seguidos</response>
/// <response code="400">Parâmetros inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpGet("following/{profileId:guid}")]
[ProducesResponseType(typeof(Pagination<List<GetFollowingResponse>>), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<Pagination<List<GetFollowingResponse>>>> GetFollowingAsync(
        [FromRoute] Guid profileId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.GetFollowingAsync(
            new GetFollowingQuery(page, perPage, query, sort),
            profileId,
            cancellationToken);

        return Ok(result);
    }

/// <summary>
///     Verifica se um perfil segue outro.
/// </summary>
/// <param name="profileId">ID do perfil</param>
/// <param name="targetProfileId">ID do perfil alvo</param>
/// <param name="wide">Contexto de eventos wide</param>
/// <param name="cancellationToken">Token de cancelamento</param>
/// <returns>True se segue, false caso contrário</returns>
/// <response code="200">Resultado da verificação</response>
/// <response code="400">Parâmetros inválidos</response>
/// <response code="401">Usuário não autenticado</response>
/// <response code="500">Erro interno do servidor</response>
[HttpGet("isfollowing/{profileId:guid}/{targetProfileId:guid}")]
[ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public async Task<ActionResult<bool>> IsFollowingAsync(
        [FromRoute] Guid profileId,
        [FromRoute] Guid targetProfileId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.IsFollowingAsync(
            new IsFollowingQuery(targetProfileId),
            profileId,
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
