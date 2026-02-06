using Fenicia.Common;
using Fenicia.Common.API;
using Fenicia.Common.Data.Requests.SocialNetwork;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

[ApiController]
[Route("[controller]")]
[Authorize]
public class FeedController(IFeedService feedService):ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetFeedAsync(CancellationToken cancellationToken, [FromQuery] PaginationQuery query)
    {
        var userId = ClaimReader.UserId(User);
        var feed = await feedService.GetFollowingFeedAsync(userId, cancellationToken, query.Page, query.PerPage);

        return Ok(feed);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync([FromBody] FeedRequest request, CancellationToken cancellationToken)
    {
        var userId = ClaimReader.UserId(User);
        var feed = await feedService.AddAsync(userId, request, cancellationToken);

        return Ok(feed);
    }
}
