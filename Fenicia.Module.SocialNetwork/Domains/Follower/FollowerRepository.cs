using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public class FollowerRepository(SocialNetworkContext context) : BaseRepository<FollowerModel>(context), IFollowerRepository
{
    public async Task<FollowerModel?> FindFollowerAsync(Guid userId, Guid followerId, CancellationToken cancellationToken)
    {
        var query = from follower in context.Followers
            where follower.IsActive && follower.UserId == userId && follower.FollowerId == followerId
            select follower;

        return await query.FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<FollowerModel>> GetFollowersAsync(Guid userId, CancellationToken cancellationToken, int page, int perPage)
    {
        var query = from follower in context.Followers
            where follower.IsActive && follower.UserId == userId
            select follower;

        return await query.ToListAsync(cancellationToken);
    }
}
