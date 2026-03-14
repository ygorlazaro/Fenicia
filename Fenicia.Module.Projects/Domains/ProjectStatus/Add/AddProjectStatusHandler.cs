using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.ProjectModels;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Add;

public class AddProjectStatusHandler(DefaultContext context)
{
    public async Task<AddProjectStatusResponse> Handle(AddProjectStatusCommand command, CancellationToken ct)
    {
        var status = new ProjectStatusModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            Name = command.Name,
            Color = command.Color,
            Order = command.Order,
            IsFinal = command.IsFinal
        };

        context.ProjectStatuses.Add(status);

        await context.SaveChangesAsync(ct);

        return new AddProjectStatusResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId);
    }
}