using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Comment;

public class CommentRepository(DefaultContext context) : Repository<CommentModel>(context)
{
    public async Task<IEnumerable<CommentModel>> GetByFeedAsync(Guid feedId, int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
            .Where(c => c.FeedId == feedId && c.ParentCommentId == null)
            .OrderBy(c => c.CommentDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<CommentModel>> GetRepliesAsync(Guid parentCommentId, int page = 1, int perPage = 10, CancellationToken ct)
    {
        return await DbSet
            .Where(c => c.ParentCommentId == parentCommentId)
            .OrderBy(c => c.CommentDate)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
