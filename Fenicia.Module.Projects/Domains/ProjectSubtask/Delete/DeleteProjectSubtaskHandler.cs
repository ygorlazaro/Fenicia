using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Delete;

public class DeleteProjectSubtaskHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = await context.ProjectSubtasks.FirstOrDefaultAsync(ps => ps.Id == command.Id, ct);

        if (projectSubtask is null)
        {
            return;
        }

        projectSubtask.Deleted = DateTime.UtcNow;

        context.ProjectSubtasks.Update(projectSubtask);

        await context.SaveChangesAsync(ct);
    }
}
