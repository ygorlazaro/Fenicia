using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.SocialNetwork;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FeedController(IFeedService feedService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetFeedAsync(CancellationToken ct, [FromQuery] PaginationQuery query)
    {
        var userId = ClaimReader.UserId(this.User);
        var feed = await feedService.GetFollowingFeedAsync(userId, ct, query.Page, query.PerPage);

        return Ok(feed);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync([FromBody] FeedRequest request, CancellationToken ct)
    {
        var userId = ClaimReader.UserId(this.User);
        var feed = await feedService.AddAsync(userId, request, ct);

        return Ok(feed);
    }
}