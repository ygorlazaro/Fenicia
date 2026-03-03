using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project.Delete;

public class DeleteProjectHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectCommand command, CancellationToken ct)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (project is null)
        {
            return;
        }

        project.Deleted = DateTime.UtcNow;

        context.Projects.Update(project);

        await context.SaveChangesAsync(ct);
    }
}
