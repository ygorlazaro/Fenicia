using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Project;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Update;

public class UpdateProjectTaskAssigneeHandler(DefaultContext context)
{
    public async Task<UpdateProjectTaskAssigneeResponse?> Handle(UpdateProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = await context.ProjectTaskAssignees.FirstOrDefaultAsync(a => a.Id == command.Id, ct);

        if (assignee is null)
        {
            return null;
        }

        assignee.TaskId = command.TaskId;
        assignee.UserId = command.UserId;
        assignee.Role = Enum.Parse<AssigneeRole>(command.Role, true);
        assignee.AssignedAt = command.AssignedAt;

        context.ProjectTaskAssignees.Update(assignee);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectTaskAssigneeResponse(
            assignee.Id,
            assignee.TaskId,
            assignee.UserId,
            assignee.Role.ToString(),
            assignee.AssignedAt,
            assignee.CompanyId);
    }
}
