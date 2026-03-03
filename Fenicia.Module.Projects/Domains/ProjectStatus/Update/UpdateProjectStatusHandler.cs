using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Update;

public class UpdateProjectStatusHandler(DefaultContext context)
{
    public async Task<UpdateProjectStatusResponse?> Handle(UpdateProjectStatusCommand command, CancellationToken ct)
    {
        var status = await context.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (status is null)
        {
            return null;
        }

        status.ProjectId = command.ProjectId;
        status.Name = command.Name;
        status.Color = command.Color;
        status.Order = command.Order;
        status.IsFinal = command.IsFinal;

        context.ProjectStatuses.Update(status);

        await context.SaveChangesAsync(ct);

        return new UpdateProjectStatusResponse(
            status.Id,
            status.ProjectId,
            status.Name,
            status.Color,
            status.Order,
            status.IsFinal,
            status.CompanyId);
    }
}
