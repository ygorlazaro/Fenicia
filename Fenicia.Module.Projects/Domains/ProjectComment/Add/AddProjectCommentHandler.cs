using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Projects.Domains.ProjectComment.Add;

public class AddProjectCommentHandler(DefaultContext context)
{
    public async Task<AddProjectCommentResponse> Handle(AddProjectCommentCommand command, CancellationToken ct)
    {
        var projectComment = new Common.Data.Models.ProjectCommentModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Content = command.Content
        };

        context.ProjectComments.Add(projectComment);

        await context.SaveChangesAsync(ct);

        return new AddProjectCommentResponse(
            projectComment.Id,
            projectComment.TaskId,
            projectComment.UserId,
            projectComment.Content,
            projectComment.CompanyId);
    }
}
