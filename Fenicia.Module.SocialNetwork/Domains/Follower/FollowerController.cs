using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.SocialNetwork;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FollowerController(IFollowerService followerService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetFollowersAsync([FromQuery] PaginationQuery query, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var followers = await followerService.GetFollowersAsync(userId, ct);
        var count = await followerService.CountAsync(userId, ct);

        var pagination = new Pagination<List<FollowerResponse>>(followers, count, query);

        return Ok(pagination);
    }
}