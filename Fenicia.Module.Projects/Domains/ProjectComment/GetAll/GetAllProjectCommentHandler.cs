using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectComment.GetAll;

public class GetAllProjectCommentHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectCommentResponse>> Handle(GetAllProjectCommentQuery query, CancellationToken ct)
    {
        return await context.ProjectComments
            .Select(pc => new GetAllProjectCommentResponse(
                pc.Id,
                pc.TaskId,
                pc.UserId,
                pc.Content,
                pc.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
