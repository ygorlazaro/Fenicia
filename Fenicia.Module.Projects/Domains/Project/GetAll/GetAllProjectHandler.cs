using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project.GetAll;

public class GetAllProjectHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectResponse>> Handle(GetAllProjectQuery query, CancellationToken ct)
    {
        return await context.Projects
            .Select(p => new GetAllProjectResponse(
                p.Id,
                p.Title,
                p.Description,
                p.Status.ToString(),
                p.StartDate,
                p.EndDate,
                p.Owner,
                p.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
