using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.GetAll;

public class GetAllProjectSubtaskHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectSubtaskResponse>> Handle(GetAllProjectSubtaskQuery query, CancellationToken ct)
    {
        return await context.ProjectSubtasks
            .Select(ps => new GetAllProjectSubtaskResponse(
                ps.Id,
                ps.TaskId,
                ps.Title,
                ps.IsCompleted,
                ps.Order,
                ps.CompletedAt,
                ps.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
