using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
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
}
