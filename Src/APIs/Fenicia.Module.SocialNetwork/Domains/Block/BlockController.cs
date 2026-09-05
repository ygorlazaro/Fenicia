using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Block.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Block;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class BlockController(BlockService blockService, IProfileService profileService) : ControllerBase
{
    [HttpPost("block")]
    [ProducesResponseType(typeof(AddBlockResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    [Consumes(MediaTypeNames.Application.Json)]
    public async Task<ActionResult<AddBlockResponse>> BlockAsync(
        [FromBody] BlockCommand command,
        WideEventContext wide,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var profileId = await GetCurrentProfileIdAsync(cancellationToken);
        var block = await blockService.BlockAsync(command, profileId, cancellationToken);

        return new CreatedResult(string.Empty, block);
    }

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
        await blockService.UnblockAsync(new UnblockCommand(blockedProfileId), profileId, cancellationToken);

        return NoContent();
    }

    [HttpGet("blocked/{profileId:guid}")]
    [ProducesResponseType(typeof(Pagination<List<GetBlockedResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<Pagination<List<GetBlockedResponse>>>> GetBlockedAsync(
        [FromRoute] Guid profileId,
        WideEventContext wide,
        [FromQuery] int page = 1,
        [FromQuery] int perPage = 10,
        [FromQuery] string? query = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        wide.UserId = ClaimReader.UserId(User).ToString();

        var result = await blockService.GetBlockedAsync(
            new GetBlockedQuery(page, perPage, query, sort),
            profileId,
            cancellationToken);

        return Ok(result);
    }

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
