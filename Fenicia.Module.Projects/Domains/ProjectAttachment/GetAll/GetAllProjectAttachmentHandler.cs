using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.GetAll;

public class GetAllProjectAttachmentHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectAttachmentResponse>> Handle(GetAllProjectAttachmentQuery query, CancellationToken ct)
    {
        return await context.ProjectAttachments
            .Select(p => new GetAllProjectAttachmentResponse(
                p.Id,
                p.TaskId,
                p.FileName,
                p.FileUrl,
                p.FileSize,
                p.UploadedBy,
                p.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
