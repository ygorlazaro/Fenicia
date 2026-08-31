using System.Net.Mime;

using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

/// <summary>
/// Gerencia operações de bloqueio de usuários.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BlockController(BlockService blockService) : ControllerBase
{
    /// <summary>
    /// Bloqueia um usuário.
    /// </summary>
    /// <param name="command">Dados do usuário a ser bloqueado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Bloqueio criado ou reativado</returns>
    /// <response code="201">Bloqueio criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a bloquear</exception>
    [HttpPost("block")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(typeof(AddBlockResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddBlockResponse>> BlockAsync([FromBody] BlockCommand command, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var block = await blockService.BlockAsync(command, ClaimReader.UserId(User), cancellationToken);

        return new CreatedResult(string.Empty, block);
    }

    /// <summary>
    /// Desbloqueia um usuário.
    /// </summary>
    /// <param name="blockedUserId">ID do usuário a ser desbloqueado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <response code="204">Bloqueio removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="403">Usuário não autorizado a desbloquear</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a desbloquear</exception>
    [HttpDelete("unblock/{blockedUserId:guid}")]
    [Authorize(Roles = "Admin")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnblockAsync([FromRoute] Guid blockedUserId, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        await blockService.UnblockAsync(new UnblockCommand(blockedUserId), ClaimReader.UserId(User), cancellationToken);

        return NoContent();
    }

    /// <summary>
    /// Obtém a lista de usuários bloqueados por um usuário.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Número da página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de usuários bloqueados</returns>
    /// <response code="200">Lista de usuários bloqueados retornada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a acessar os bloqueados</exception>
    [HttpGet("blocked/{userId:guid}")]
    [ProducesResponseType(typeof(Pagination<List<GetBlockedResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetBlockedResponse>>>> GetBlockedAsync([FromRoute] Guid userId, WideEventContext wide, [FromQuery] int page = 1, [FromQuery] int perPage = 10, [FromQuery] string? query = null, [FromQuery] string? sort = null, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await blockService.GetBlockedAsync(new GetBlockedQuery(page, perPage, query, sort), userId, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Verifica se um usuário está bloqueando outro.
    /// </summary>
    /// <param name="userId">ID do usuário</param>
    /// <param name="blockedUserId">ID do usuário bloqueado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Verdadeiro se está bloqueando</returns>
    /// <response code="200">Verificação realizada com sucesso</response>
    /// <response code="400">ID inválido</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    /// <exception cref="UnauthorizedAccessException">Usuário não autorizado a verificar o bloqueio</exception>
    [HttpGet("isblocked/{userId:guid}/{blockedUserId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsBlockedAsync([FromRoute] Guid userId, [FromRoute] Guid blockedUserId, WideEventContext wide, CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await blockService.IsBlockedAsync(new IsBlockedQuery(blockedUserId), userId, cancellationToken);

        return Ok(result);
    }
}
