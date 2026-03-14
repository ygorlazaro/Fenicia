using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Add;

public class AddProjectTaskAssigneeHandler(DefaultContext context)
{
    public async Task<AddProjectTaskAssigneeResponse> Handle(AddProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = new TaskAssigneeModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            UserId = command.UserId,
            Role = Enum.Parse<EnumAssigneeRole>(command.Role, true),
            AssignedAt = command.AssignedAt
        };

        context.ProjectTaskAssignees.Add(assignee);

        await context.SaveChangesAsync(ct);

        return new AddProjectTaskAssigneeResponse(assignee.Id, assignee.TaskId, assignee.UserId, assignee.Role.ToString(), assignee.AssignedAt, assignee.CompanyId);
    }
}