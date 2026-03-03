using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment.GetById;

public class GetProjectCommentByIdHandler(DefaultContext context)
{
    public async Task<GetProjectCommentByIdResponse?> Handle(GetProjectCommentByIdQuery query, CancellationToken ct)
    {
        var projectComment = await context.ProjectComments
            .FirstOrDefaultAsync(pc => pc.Id == query.Id, ct);

        if (projectComment is null)
        {
            return null;
        }

        return new GetProjectCommentByIdResponse(
            projectComment.Id,
            projectComment.TaskId,
            projectComment.UserId,
            projectComment.Content,
            projectComment.CompanyId);
    }
}
