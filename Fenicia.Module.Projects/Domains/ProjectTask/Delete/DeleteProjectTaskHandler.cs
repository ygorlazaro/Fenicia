using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Delete;

public class DeleteProjectTaskHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectTaskCommand command, CancellationToken ct)
    {
        var projectTask = await context.ProjectTasks.FirstOrDefaultAsync(pt => pt.Id == command.Id, ct);

        if (projectTask is null)
        {
            return;
        }

        projectTask.Deleted = DateTime.UtcNow;

        context.ProjectTasks.Update(projectTask);

        await context.SaveChangesAsync(ct);
    }
}
