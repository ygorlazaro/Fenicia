using System.Net.Mime;
using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;
using Fenicia.Module.SocialNetwork.Domains.Profile.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship;

[Authorize]
[ApiController]
[Route("[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
public class FriendshipController(FriendshipService friendshipService, IProfileService profileService) : ControllerBase
{
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
