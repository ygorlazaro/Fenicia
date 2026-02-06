using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedRepository(SocialNetworkContext context) : BaseRepository<FeedModel>(context), IFeedRepository
{
    public async Task<List<FeedModel>> GetFollowingFeedAsync(Guid userId, CancellationToken cancellationToken, int page = 1, int perPage = 10)
    {
        var query = from feed in context.Feeds
            join userFollowing in context.Users on feed.UserId equals userFollowing.UserId
            join userFollower in context.Followers on userFollowing.UserId equals userFollower.FollowerId
            where userFollower.UserId == userId || feed.UserId == userId
            orderby feed.Date descending
            select feed;

        return await query.Skip((page - 1) * perPage).Take(perPage).ToListAsync(cancellationToken);
    }
}
