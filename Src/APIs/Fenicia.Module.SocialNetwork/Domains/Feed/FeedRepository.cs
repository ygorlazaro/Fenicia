using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedRepository(DefaultContext context) : Repository<FeedModel>(context)
{
    internal new async Task<IEnumerable<FeedModel>> GetAllAsync(
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(f => f.Profile)
            .ThenInclude(p => p.Upload)
            .OrderByDescending(f => f.Date)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }

    internal Task<FeedModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(f => f.Profile)
            .ThenInclude(p => p.Upload)
            .Include(f => f.Comments)
            .Include(f => f.Likes)
            .Include(f => f.Shares)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);
    }
}