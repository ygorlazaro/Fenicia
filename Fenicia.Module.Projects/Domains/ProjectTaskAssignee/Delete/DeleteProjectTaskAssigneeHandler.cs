using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Delete;

public class DeleteProjectTaskAssigneeHandler(DefaultContext context)
{
    public async Task Handle(DeleteProjectTaskAssigneeCommand command, CancellationToken ct)
    {
        var assignee = await context.ProjectTaskAssignees.FirstOrDefaultAsync(a => a.Id == command.Id, ct);

        if (assignee is null)
        {
            return;
        }

        assignee.Deleted = DateTime.UtcNow;

        context.ProjectTaskAssignees.Update(assignee);

        await context.SaveChangesAsync(ct);
    }
}
