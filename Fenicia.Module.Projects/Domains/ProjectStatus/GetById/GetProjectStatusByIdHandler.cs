using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.GetById;

public class GetProjectStatusByIdHandler(DefaultContext context)
{
    public async Task<GetProjectStatusByIdResponse?> Handle(GetProjectStatusByIdQuery query, CancellationToken ct)
    {
        var status = await context.ProjectStatuses
            .FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        if (status is null)
        {
            return null;
        }

        return new GetProjectStatusByIdResponse(
            status.Id,
            status.ProjectId,
            status.Name,
            status.Color,
            status.Order,
            status.IsFinal,
            status.CompanyId);
    }
}
