using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Update;

public class UpdateProjectAttachmentHandler(DefaultContext context)
{
    public async Task<UpdateProjectAttachmentResponse?> Handle(UpdateProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = await context.ProjectAttachments.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (projectAttachment is null)
        {
            return null;
        }

        projectAttachment.TaskId = command.TaskId;
        projectAttachment.FileName = command.FileName;
        projectAttachment.FileUrl = command.FileUrl;
        projectAttachment.FileSize = command.FileSize;
        projectAttachment.UploadedBy = command.UploadedBy;

        context.ProjectAttachments.Update(projectAttachment);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectAttachmentResponse(
            projectAttachment.Id,
            projectAttachment.TaskId,
            projectAttachment.FileName,
            projectAttachment.FileUrl,
            projectAttachment.FileSize,
            projectAttachment.UploadedBy,
            projectAttachment.CompanyId);
    }
}
