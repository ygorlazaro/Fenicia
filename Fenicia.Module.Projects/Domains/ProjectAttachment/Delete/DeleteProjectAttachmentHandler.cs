using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectAttachment.Delete;

public class DeleteProjectAttachmentHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectAttachmentCommand command, CancellationToken ct)
    {
        var projectAttachment = await context.ProjectAttachments.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (projectAttachment is null)
        {
            return;
        }

        projectAttachment.Deleted = DateTime.UtcNow;

        context.ProjectAttachments.Update(projectAttachment);

        await context.SaveChangesAsync(ct);
    }
}
