using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.GetAll;

public class GetAllProjectStatusHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectStatusResponse>> Handle(GetAllProjectStatusQuery query, CancellationToken ct)
    {
        return await context.ProjectStatuses
            .Select(s => new GetAllProjectStatusResponse(
                s.Id,
                s.ProjectId,
                s.Name,
                s.Color,
                s.Order,
                s.IsFinal,
                s.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
