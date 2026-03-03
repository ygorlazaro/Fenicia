using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetAll;

public class GetAllProjectTaskAssigneeHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectTaskAssigneeResponse>> Handle(GetAllProjectTaskAssigneeQuery query, CancellationToken ct)
    {
        return await context.ProjectTaskAssignees
            .Select(a => new GetAllProjectTaskAssigneeResponse(
                a.Id,
                a.TaskId,
                a.UserId,
                a.Role.ToString(),
                a.AssignedAt,
                a.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
