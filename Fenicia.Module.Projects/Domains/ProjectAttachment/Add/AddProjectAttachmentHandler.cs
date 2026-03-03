using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Add;

public class AddProjectAttachmentHandler(DefaultContext context)
{
    public async Task<AddProjectAttachmentResponse> Handle(AddProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = new Common.Data.Models.ProjectAttachment
        {
            Id = command.Id,
            TaskId = command.TaskId,
            FileName = command.FileName,
            FileUrl = command.FileUrl,
            FileSize = command.FileSize,
            UploadedBy = command.UploadedBy,
            ContentType = command.ContentType,
        };

        context.ProjectAttachments.Add(projectAttachment);

        await context.SaveChangesAsync(ct);

        return new AddProjectAttachmentResponse(
            projectAttachment.Id,
            projectAttachment.TaskId,
            projectAttachment.FileName,
            projectAttachment.FileUrl,
            projectAttachment.FileSize,
            projectAttachment.UploadedBy,
            projectAttachment.CompanyId);
    }
}
