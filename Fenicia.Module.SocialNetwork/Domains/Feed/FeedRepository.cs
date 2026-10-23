using Fenicia.Common;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetwork;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.SocialNetwork.Domains.Feed.Interfaces;
using Microsoft.EntityFrameworkCore;

/// <summary>
/// Fenicia.Module.SocialNetwork/Domains/Feed/FeedRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.SocialNetwork.Domains.Feed;

public class FeedRepository(DefaultContext context) : Repository<FeedModel>(context), IFeedRepository
{
    public override async Task<IEnumerable<FeedModel>> GetAllAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(f => f.Profile)
            .ThenInclude(p => p.Upload)
            .OrderByDescending(f => f.Date)
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }

    public Task<FeedModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default)
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
