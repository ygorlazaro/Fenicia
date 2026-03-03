using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Update;

public class UpdateProjectCommentHandler(DefaultContext context)
{
    public async Task<UpdateProjectCommentResponse?> Handle(UpdateProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = await context.ProjectComments.FirstOrDefaultAsync(pc => pc.Id == command.Id, ct);

        if (projectComment is null)
        {
            return null;
        }

        projectComment.Content = command.Content;

        context.ProjectComments.Update(projectComment);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectCommentResponse(
            projectComment.Id,
            projectComment.TaskId,
            projectComment.UserId,
            projectComment.Content,
            projectComment.CompanyId);
    }
}
