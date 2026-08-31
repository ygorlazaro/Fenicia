using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeRepository(DefaultContext context) : Repository<LikeModel>(context)
{
    public async Task<LikeModel?> GetByUserAndFeedAsync(Guid userId, Guid feedId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(e => e.UserId == userId && e.FeedId == feedId, cancellationToken);
    }

    public async Task<IEnumerable<LikeModel>> GetByFeedAsync(Guid feedId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.FeedId == feedId)
            .OrderByDescending(e => e.LikeDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }
}
