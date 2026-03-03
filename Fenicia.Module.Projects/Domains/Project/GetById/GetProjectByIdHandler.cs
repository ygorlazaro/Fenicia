using Fenicia.Common.Data.Contexts;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project.GetById;

public class GetProjectByIdHandler(DefaultContext context)
{
    public async Task<GetProjectByIdResponse?> Handle(GetProjectByIdQuery query, CancellationToken ct)
    {
        var project = await context.Projects
            .Include(p => p.Statuses)
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        if (project is null)
        {
            return null;
        }

        return new GetProjectByIdResponse(
            project.Id,
            project.Title,
            project.Description,
            project.Status.ToString(),
            project.StartDate,
            project.EndDate,
            project.Owner,
            project.CompanyId,
            project.Statuses.Select(s => new ProjectStatusResponse(
                s.Id,
                s.Name,
                s.Color,
                s.Order,
                s.IsFinal)).ToList(),
            project.Tasks.Select(t => new ProjectTaskResponse(
                t.Id,
                t.Title,
                t.Description,
                t.Priority.ToString(),
                t.Type.ToString(),
                t.EstimatePoints,
                t.DueDate)).ToList());
    }
}
