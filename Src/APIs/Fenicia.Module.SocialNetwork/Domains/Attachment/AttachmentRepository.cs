using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.SocialNetworkModels;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.SocialNetwork.Domains.Attachment;

public class AttachmentRepository(DefaultContext context) : Repository<AttachmentModel>(context)
{
    public async Task<IEnumerable<AttachmentModel>> GetByCommentAsync(int page, int perPage, Guid commentId, CancellationToken ct)
    {
        return await DbSet
                .Where(e => e.CommentId == commentId)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(ct);
    }
}
