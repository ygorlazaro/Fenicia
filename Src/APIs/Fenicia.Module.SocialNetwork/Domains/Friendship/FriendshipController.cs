using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

/// <summary>
/// Gerencia operações de amizades e seguidores.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FriendshipController(FriendshipService friendshipService) : ControllerBase
{
    /// <summary>
    /// Segue um usuário.
    /// </summary>
    /// <param name="command">Dados do usuário a ser seguido</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Amizade criada ou reativada</returns>
    /// <response code="201">Amizade criada com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a seguir</exception>
    [HttpPost("follow")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AddFriendshipResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddFriendshipResponse>> FollowAsync([FromBody] FollowCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var friendship = await friendshipService.FollowAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, friendship);
    }

    /// <summary>
    /// Deixa de seguir um usuário.
    /// </summary>
    /// <param name="targetUserId">ID do usuário a deixar de seguir</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <response code="204">Amizade removida com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a deixar de seguir</exception>
    [HttpDelete("unfollow/{targetUserId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnfollowAsync([FromRoute] Guid targetUserId, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await friendshipService.UnfollowAsync(new UnfollowCommand(targetUserId), ClaimReader.UserId(User), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Obtém a lista de seguidores de um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de seguidores</returns>
    /// <response code="200">Lista de seguidores retornada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os seguidores</exception>
    [HttpGet("followers/{userId:guid}")]
    [ProducesResponseType(typeof(Pagination<List<GetFollowersResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetFollowersResponse>>>> GetFollowersAsync([FromRoute] Guid userId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.GetFollowersAsync(new GetFollowersQuery(page, perPage), userId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Obtém a lista de usuários que um usuário está seguindo.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários seguidos</returns>
    /// <response code="200">Lista de usuários seguidos retornada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os seguidos</exception>
    [HttpGet("following/{userId:guid}")]
    [ProducesResponseType(typeof(Pagination<List<GetFollowingResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetFollowingResponse>>>> GetFollowingAsync([FromRoute] Guid userId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.GetFollowingAsync(new GetFollowingQuery(page, perPage), userId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Verifica se um usuário está seguindo outro.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="targetUserId">ID do usuário alvo</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Verdadeiro se está seguindo</returns>
    /// <response code="200">Verificação realizada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a verificar o relacionamento</exception>
    [HttpGet("isfollowing/{userId:guid}/{targetUserId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsFollowingAsync([FromRoute] Guid userId, [FromRoute] Guid targetUserId, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await friendshipService.IsFollowingAsync(new IsFollowingQuery(targetUserId), userId, cancellationToken);

        return Ok(result);
    }
}
