using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Delete;

public class DeleteProjectStatusHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectStatusCommand command, CancellationToken ct)
    {
        var status = await context.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (status is null)
        {
            return;
        }

        status.Deleted = DateTime.UtcNow;

        context.ProjectStatuses.Update(status);

        await context.SaveChangesAsync(ct);
    }
}
