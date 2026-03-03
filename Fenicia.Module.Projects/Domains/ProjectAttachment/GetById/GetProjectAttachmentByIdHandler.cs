using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.GetById;

public class GetProjectAttachmentByIdHandler(DefaultContext context)
{
    public async Task<GetProjectAttachmentByIdResponse?> Handle(GetProjectAttachmentByIdQuery query, CancellationToken ct)
    {
        var projectAttachment = await context.ProjectAttachments
            .FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (projectAttachment is null)
        {
            return null;
        }

        return new GetProjectAttachmentByIdResponse(
            projectAttachment.Id,
            projectAttachment.TaskId,
            projectAttachment.FileName,
            projectAttachment.FileUrl,
            projectAttachment.FileSize,
            projectAttachment.UploadedBy,
            projectAttachment.CompanyId);
    }
}
