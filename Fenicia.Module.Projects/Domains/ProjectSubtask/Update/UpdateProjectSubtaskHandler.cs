using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Update;

public class UpdateProjectSubtaskHandler(DefaultContext context)
{
    public async Task<UpdateProjectSubtaskResponse?> Handle(UpdateProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = await context.ProjectSubtasks.FirstOrDefaultAsync(ps => ps.Id == command.Id, ct);

        if (projectSubtask is null)
        {
            return null;
        }

        projectSubtask.TaskId = command.TaskId;
        projectSubtask.Title = command.Title;
        projectSubtask.IsCompleted = command.IsCompleted;
        projectSubtask.Order = command.Order;
        projectSubtask.CompletedAt = command.CompletedAt;

        context.ProjectSubtasks.Update(projectSubtask);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectSubtaskResponse(
            projectSubtask.Id,
            projectSubtask.TaskId,
            projectSubtask.Title,
            projectSubtask.IsCompleted,
            projectSubtask.Order,
            projectSubtask.CompletedAt,
            projectSubtask.CompanyId);
    }
}
