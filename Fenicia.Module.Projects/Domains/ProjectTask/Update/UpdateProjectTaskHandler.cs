using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Project;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Update;

public class UpdateProjectTaskHandler(DefaultContext context)
{
    public async Task<UpdateProjectTaskResponse?> Handle(UpdateProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = await context.ProjectTasks.FirstOrDefaultAsync(pt => pt.Id == command.Id, ct);

        if (projectTask is null)
        {
            return null;
        }

        projectTask.ProjectId = command.ProjectId;
        projectTask.StatusId = command.StatusId;
        projectTask.Title = command.Title;
        projectTask.Description = command.Description;
        projectTask.Priority = Enum.Parse<TaskPriority>(command.Priority, true);
        projectTask.Type = Enum.Parse<TaskType>(command.Type, true);
        projectTask.Order = command.Order;
        projectTask.EstimatePoints = command.EstimatePoints;
        projectTask.DueDate = command.DueDate;
        projectTask.CreatedBy = command.CreatedBy;

        context.ProjectTasks.Update(projectTask);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectTaskResponse(
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
