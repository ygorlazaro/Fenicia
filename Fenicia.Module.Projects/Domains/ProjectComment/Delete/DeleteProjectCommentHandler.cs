using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Delete;

public class DeleteProjectCommentHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = await context.ProjectComments.FirstOrDefaultAsync(pc => pc.Id == command.Id, ct);

        if (projectComment is null)
        {
            return;
        }

        projectComment.Deleted = DateTime.UtcNow;

        context.ProjectComments.Update(projectComment);

        await context.SaveChangesAsync(ct);
    }
}
