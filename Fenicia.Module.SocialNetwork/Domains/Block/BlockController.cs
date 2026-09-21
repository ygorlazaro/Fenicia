using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.DTOs.SocialNetwork.Block;
using Fenicia.Module.SocialNetwork.Domains.Block.Interfaces;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

/// <summary>
///     Gerencia operações de bloqueio entre perfis.
/// </summary>
[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BlockController(IBlockService blockService, IProfileService profileService) : ControllerBase
{
    /// <summary>
    ///     Bloqueia um perfil.
    /// </summary>
    /// <param name="command">Dados do bloqueio</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Bloqueio criado ou reativado</returns>
    /// <response code="201">Bloqueio criado com sucesso</response>
    /// <response code="400">Dados inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpPost("block")]
    [ProducesResponseType(typeof(BlockResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<BlockResponse>> BlockAsync(
        [FromBody] BlockRequest command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var block = await blockService.BlockAsync(command, profileId, cancellationToken);

        return new CreatedResult(string.Empty, block);
    }

    /// <summary>
    ///     Remove o bloqueio de um perfil.
    /// </summary>
    /// <param name="blockedProfileId">ID do perfil bloqueado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Sem conteúdo</returns>
    /// <response code="204">Bloqueio removido com sucesso</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpDelete("unblock/{blockedProfileId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult> UnblockAsync(
        [FromRoute] Guid blockedProfileId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        await blockService.UnblockAsync(new BlockRequest(blockedProfileId), profileId, cancellationToken);

        return NoContent();
    }

    /// <summary>
    ///     Obtém a lista de perfis bloqueados.
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="page">Página</param>
    /// <param name="perPage">Itens por página</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>Lista paginada de perfis bloqueados</returns>
    /// <response code="200">Lista de perfis bloqueados</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("blocked/{profileId:guid}")]
    [ProducesResponseType(typeof(Pagination<List<BlockResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<BlockResponse>>>> GetBlockedAsync(
        [FromRoute] Guid profileId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await blockService.GetBlockedAsync(profileId, page, perPage, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    ///     Verifica se um perfil está bloqueado.
    /// </summary>
    /// <param name="profileId">ID do perfil</param>
    /// <param name="blockedProfileId">ID do perfil bloqueado</param>
    /// <param name="wide">Contexto de eventos wide</param>
    /// <param name="cancellationToken">Token de cancelamento</param>
    /// <returns>True se estiver bloqueado, false caso contrário</returns>
    /// <response code="200">Resultado da verificação</response>
    /// <response code="400">Parâmetros inválidos</response>
    /// <response code="401">Usuário não autenticado</response>
    /// <response code="500">Erro interno do servidor</response>
    [HttpGet("isblocked/{profileId:guid}/{blockedProfileId:guid}")]
    [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<bool>> IsBlockedAsync(
        [FromRoute] Guid profileId,
        [FromRoute] Guid blockedProfileId,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await blockService.IsBlockedAsync(new IsBlockedQuery(blockedProfileId), profileId, cancellationToken);

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
