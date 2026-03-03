using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

public class AddProjectTaskAssigneeHandler(DefaultContext context)
{
    public async Task<AddProjectTaskAssigneeResponse> Handle(AddProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = new Common.Data.Models.ProjectTaskAssigneeModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Role = Enum.Parse<AssigneeRole>(command.Role, true),
            AssignedAt = command.AssignedAt
        };

        context.ProjectTaskAssignees.Add(assignee);

        await context.SaveChangesAsync(ct);

        return new AddProjectTaskAssigneeResponse(
            assignee.Id,
            assignee.TaskId,
            assignee.UserId,
            assignee.Role.ToString(),
            assignee.AssignedAt,
            assignee.CompanyId);
    }
}
