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
}
