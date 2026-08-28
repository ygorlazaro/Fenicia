using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectSubtask.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask;

public class ProjectSubtaskService(DefaultContext db)
{
    public async Task<List<GetAllProjectSubtaskResponse>> GetAllAsync(GetAllProjectSubtaskQuery query, CancellationToken ct)
    {
        return await db.ProjectSubtasks.Select(ps => new GetAllProjectSubtaskResponse(ps.Id, ps.TaskId, ps.Title, ps.IsCompleted, ps.Order, ps.CompletedAt, ps.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectSubtaskByIdResponse?> GetByIdAsync(GetProjectSubtaskByIdQuery query, CancellationToken ct)
    {
        var projectSubtask = await db.ProjectSubtasks.FirstOrDefaultAsync(ps => ps.Id == query.Id, ct);

        return projectSubtask switch
        {
            null => null,
            _ => new GetProjectSubtaskByIdResponse(projectSubtask.Id, projectSubtask.TaskId, projectSubtask.Title, projectSubtask.IsCompleted, projectSubtask.Order, projectSubtask.CompletedAt, projectSubtask.CompanyId)
        };
    }

    public async Task<AddProjectSubtaskResponse> AddAsync(AddProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = new ProjectSubtaskModel
        {
            Id = command.Id,
            TaskId = command.TaskId,
            Title = command.Title,
            IsCompleted = command.IsCompleted,
            Order = command.Order,
            CompletedAt = command.CompletedAt
        };

        db.ProjectSubtasks.Add(projectSubtask);

        await db.SaveChangesAsync(ct);

        return new AddProjectSubtaskResponse(projectSubtask.Id, projectSubtask.TaskId, projectSubtask.Title, projectSubtask.IsCompleted, projectSubtask.Order, projectSubtask.CompletedAt, projectSubtask.CompanyId);
    }

    public async Task<UpdateProjectSubtaskResponse?> UpdateAsync(UpdateProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = await db.ProjectSubtasks.FirstOrDefaultAsync(ps => ps.Id == command.Id, ct);

        if (projectSubtask is null)
        {
            return null;
        }

        projectSubtask.TaskId = command.TaskId;
        projectSubtask.Title = command.Title;
        projectSubtask.IsCompleted = command.IsCompleted;
        projectSubtask.Order = command.Order;
        projectSubtask.CompletedAt = command.CompletedAt;

        db.ProjectSubtasks.Update(projectSubtask);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectSubtaskResponse(projectSubtask.Id, projectSubtask.TaskId, projectSubtask.Title, projectSubtask.IsCompleted, projectSubtask.Order, projectSubtask.CompletedAt, projectSubtask.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectSubtaskCommand command, CancellationToken ct)
    {
        var projectSubtask = await db.ProjectSubtasks.FirstOrDefaultAsync(ps => ps.Id == command.Id, ct);

        if (projectSubtask is null)
        {
            return;
        }

        projectSubtask.Deleted = DateTime.UtcNow;

        db.ProjectSubtasks.Update(projectSubtask);

        await db.SaveChangesAsync(ct);
    }
}
