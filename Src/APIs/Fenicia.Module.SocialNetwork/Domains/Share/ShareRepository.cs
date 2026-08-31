using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Share;

public class ShareRepository(DefaultContext context) : Repository<ShareModel>(context)
{
    public async Task<IEnumerable<ShareModel>> GetSharesByFeedAsync(int page, int perPage, Guid feedId, CancellationToken ct)
    {
        return await DbSet
                .Where(e => e.OriginalFeedId == feedId && e.Deleted == null)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
