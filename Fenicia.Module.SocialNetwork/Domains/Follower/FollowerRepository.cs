using Fenicia.Common.Data.Abstracts;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Follower;

public class FollowerRepository(SocialNetworkContext context) : BaseRepository<FollowerModel>(context), IFollowerRepository
{
    public async Task<FollowerModel?> FindFollowerAsync(Guid userId, Guid followerId, CancellationToken ct)
    {
        var query = context.Followers.Where(f => f.IsActive && f.UserId == userId && f.FollowerId == followerId);

        return await query.FirstOrDefaultAsync(ct);
    }

    public async Task<List<FollowerModel>> GetFollowersAsync(Guid userId, CancellationToken ct, int page, int perPage)
    {
        var query = context.Followers.Where(f => f.IsActive && f.UserId == userId);

        return await query.ToListAsync(ct);
    }
}