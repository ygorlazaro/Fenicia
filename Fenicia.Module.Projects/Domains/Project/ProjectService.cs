using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.Project.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Project;

public class ProjectService(DefaultContext db)
{
    public async Task<List<GetAllProjectResponse>> GetAllAsync(GetAllProjectQuery query, CancellationToken ct)
    {
        return await db.Projects.Select(p => new GetAllProjectResponse(p.Id, p.Title, p.Description, p.Status.ToString(), p.StartDate, p.EndDate, p.Owner, p.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectByIdResponse?> GetByIdAsync(GetProjectByIdQuery query, CancellationToken ct)
    {
        var project = await db.Projects.Include(p => p.Statuses).Include(p => p.Tasks).FirstOrDefaultAsync(p => p.Id == query.Id, ct);

        return project switch
        {
            null => null,
            _ => new GetProjectByIdResponse(project.Id, project.Title, project.Description, project.Status.ToString(), project.StartDate, project.EndDate, project.Owner, project.CompanyId, project.Statuses.Select(s => new ProjectStatusResponse(s.Id, s.Name, s.Color, s.Order, s.IsFinal)).ToList(), project.Tasks.Select(t => new ProjectTaskResponse(t.Id, t.Title, t.Description, t.Priority.ToString(), t.Type.ToString(), t.EstimatePoints, t.DueDate)).ToList())
        };
    }

    public async Task<AddProjectResponse> AddAsync(AddProjectCommand command, CancellationToken ct)
    {
        var project = new ProjectModel
        {
            Id = command.Id,
            Title = command.Title,
            Description = command.Description,
            Status = Enum.Parse<EnumProjectStatus>(command.Status, true),
            StartDate = command.StartDate,
            EndDate = command.EndDate,
            Owner = command.Owner
        };

        db.Projects.Add(project);

        await db.SaveChangesAsync(ct);

        return new AddProjectResponse(project.Id, project.Title, project.Description, project.Status.ToString(), project.StartDate, project.EndDate, project.Owner, project.CompanyId);
    }

    public async Task<UpdateProjectResponse?> UpdateAsync(UpdateProjectCommand command, CancellationToken ct)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (project is null)
        {
            return null;
        }

        project.Title = command.Title;
        project.Description = command.Description;
        project.Status = Enum.Parse<EnumProjectStatus>(command.Status, true);
        project.StartDate = command.StartDate;
        project.EndDate = command.EndDate;
        project.Owner = command.Owner;

        db.Projects.Update(project);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectResponse(project.Id, project.Title, project.Description, project.Status.ToString(), project.StartDate, project.EndDate, project.Owner, project.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectCommand command, CancellationToken ct)
    {
        var project = await db.Projects.FirstOrDefaultAsync(p => p.Id == command.Id, ct);

        if (project is null)
        {
            return;
        }

        project.Deleted = DateTime.UtcNow;

        db.Projects.Update(project);

        await db.SaveChangesAsync(ct);
    }
}
