using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Add;

public class AddProjectSubtaskHandler(DefaultContext context)
{
    public async Task<AddProjectSubtaskResponse> Handle(AddProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = new Common.Data.Models.ProjectSubtaskModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            Title = command.Title,
            IsCompleted = command.IsCompleted,
            Order = command.Order,
            CompletedAt = command.CompletedAt
        };

        context.ProjectSubtasks.Add(projectSubtask);

        await context.SaveChangesAsync(ct);

        return new AddProjectSubtaskResponse(
            projectSubtask.Id,
            projectSubtask.TaskId,
            projectSubtask.Title,
            projectSubtask.IsCompleted,
            projectSubtask.Order,
            projectSubtask.CompletedAt,
            projectSubtask.CompanyId);
    }
}
