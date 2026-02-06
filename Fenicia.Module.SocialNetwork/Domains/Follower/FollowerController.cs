using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Responses.SocialNetwork;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FollowerController(IFollowerService followerService):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFollowersAsync([FromQuery] PaginationQuery query, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var followers = await followerService.GetFollowersAsync(userId, cancellationToken);
        var count = await followerService.CountAsync(userId, cancellationToken);

        var pagination = new Pagination<List<FollowerResponse>>(followers, count, query);

        return Ok(pagination);
    }
}
