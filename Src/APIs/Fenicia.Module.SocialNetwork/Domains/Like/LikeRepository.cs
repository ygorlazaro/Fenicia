using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Like;

public class LikeRepository(DefaultContext context) : Repository<LikeModel>(context)
{
    public Task<LikeModel?> GetByProfileAndFeedAsync(
        Guid profileId,
        Guid feedId,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .FirstOrDefaultAsync(e => e.ProfileId == profileId && e.FeedId == feedId, cancellationToken);
    }

    public Task<List<LikeModel>> GetByProfileIdAsync(
        Guid profileId,
        int page,
        int perPage,
        CancellationToken cancellationToken = default)
    {
        return DbSet
            .Where(l => l.ProfileId == profileId)
            .OrderByDescending(l => l.LikeDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }
}
