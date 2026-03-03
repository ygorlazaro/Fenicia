using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectTask.GetAll;

public class GetAllProjectTaskHandler(DefaultContext context)
{
    public async Task<List<GetAllProjectTaskResponse>> Handle(GetAllProjectTaskQuery query, CancellationToken ct)
    {
        return await context.ProjectTasks
            .Select(pt => new GetAllProjectTaskResponse(
                pt.Id,
                pt.ProjectId,
                pt.StatusId,
                pt.Title,
                pt.Description,
                pt.Priority.ToString(),
                pt.Type.ToString(),
                pt.Order,
                pt.EstimatePoints,
                pt.DueDate,
                pt.CreatedBy,
                pt.CompanyId))
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(ct);
    }
}
