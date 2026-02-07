using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedRepository(SocialNetworkContext context) : BaseRepository<FeedModel>(context), IFeedRepository
{
    public async Task<List<FeedModel>> GetFollowingFeedAsync(Guid userId, CancellationToken ct, int page = 1, int perPage = 10)
    {
        var query = from f in context.Feeds
            join following in context.Users on f.UserId equals following.UserId
            join follower in context.Followers on following.UserId equals follower.FollowerId
            where follower.UserId == userId || f.UserId == userId
            orderby f.Date descending
            select f;

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(ct);
    }
}
