using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Fenicia.Common.Data.Models.ProjectModels;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusService(DefaultContext db)
{
    public async Task<List<GetAllProjectStatusResponse>> GetAllAsync(GetAllProjectStatusQuery query, CancellationToken ct)
    {
        return await db.ProjectStatuses.Select(s => new GetAllProjectStatusResponse(s.Id, s.ProjectId, s.Name, s.Color, s.Order, s.IsFinal, s.CompanyId)).Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(ct);
    }

    public async Task<GetProjectStatusByIdResponse?> GetByIdAsync(GetProjectStatusByIdQuery query, CancellationToken ct)
    {
        var status = await db.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == query.Id, ct);

        return status switch
        {
            null => null,
            _ => new GetProjectStatusByIdResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId)
        };
    }

    public async Task<AddProjectStatusResponse> AddAsync(AddProjectStatusCommand command, CancellationToken ct)
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

        db.ProjectStatuses.Add(status);

        await db.SaveChangesAsync(ct);

        return new AddProjectStatusResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId);
    }

    public async Task<UpdateProjectStatusResponse?> UpdateAsync(UpdateProjectStatusCommand command, CancellationToken ct)
    {
        var status = await db.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (status is null)
        {
            return null;
        }

        status.ProjectId = command.ProjectId;
        status.Name = command.Name;
        status.Color = command.Color;
        status.Order = command.Order;
        status.IsFinal = command.IsFinal;

        db.ProjectStatuses.Update(status);

        await db.SaveChangesAsync(ct);

        return new UpdateProjectStatusResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectStatusCommand command, CancellationToken ct)
    {
        var status = await db.ProjectStatuses.FirstOrDefaultAsync(s => s.Id == command.Id, ct);

        if (status is null)
        {
            return;
        }

        status.Deleted = DateTime.UtcNow;

        db.ProjectStatuses.Update(status);

        await db.SaveChangesAsync(ct);
    }
}
