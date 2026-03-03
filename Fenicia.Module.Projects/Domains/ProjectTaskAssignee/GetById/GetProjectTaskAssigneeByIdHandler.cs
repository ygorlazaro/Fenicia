using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.GetById;

public class GetProjectTaskAssigneeByIdHandler(DefaultContext context)
{
    public async Task<GetProjectTaskAssigneeByIdResponse?> Handle(GetProjectTaskAssigneeByIdQuery query, CancellationToken ct)
    {
        var assignee = await context.ProjectTaskAssignees
            .FirstOrDefaultAsync(a => a.Id == query.Id, ct);

        if (assignee is null)
        {
            return null;
        }

        return new GetProjectTaskAssigneeByIdResponse(
            assignee.Id,
            assignee.TaskId,
            assignee.UserId,
            assignee.Role.ToString(),
            assignee.AssignedAt,
            assignee.CompanyId);
    }
}
