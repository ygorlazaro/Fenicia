using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Add;

public class AddProjectTaskHandler(DefaultContext context)
{
    public async Task<AddProjectTaskResponse> Handle(AddProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = new Common.Data.Models.ProjectTaskModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            StatusId = command.StatusId,
            Title = command.Title,
            Description = command.Description,
            Priority = Enum.Parse<TaskPriority>(command.Priority, true),
            Type = Enum.Parse<TaskType>(command.Type, true),
            Order = command.Order,
            EstimatePoints = command.EstimatePoints,
            DueDate = command.DueDate,
            CreatedBy = command.CreatedBy
        };

        context.ProjectTasks.Add(projectTask);

        await context.SaveChangesAsync(ct);

        return new AddProjectTaskResponse(
            projectTask.Id,
            projectTask.ProjectId,
            projectTask.StatusId,
            projectTask.Title,
            projectTask.Description,
            projectTask.Priority.ToString(),
            projectTask.Type.ToString(),
            projectTask.Order,
            projectTask.EstimatePoints,
            projectTask.DueDate,
            projectTask.CreatedBy,
            projectTask.CompanyId);
    }
}
