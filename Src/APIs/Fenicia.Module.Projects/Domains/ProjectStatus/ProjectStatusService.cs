using Fenicia.Common;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusService(IProjectStatusRepository repository) : IProjectStatusService
{
    public async Task<List<GetAllProjectStatusResponse>> GetAllAsync(GetAllProjectStatusQuery query, CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var filters = AdvancedQueryParser.Parse(query.Query);
        var filteredQuery = baseQuery.ApplyAdvancedQuery(filters, query.Sort);
        var statuses = await filteredQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage).ToListAsync(cancellationToken);
        return [.. statuses.Select(s => new GetAllProjectStatusResponse(s.Id, s.ProjectId, s.Name, s.Color, s.Order, s.IsFinal, s.CompanyId))];
    }

    public async Task<GetProjectStatusByIdResponse?> GetByIdAsync(GetProjectStatusByIdQuery query, CancellationToken cancellationToken = default)
    {
        var status = await repository.GetByIdAsync(query.Id, cancellationToken);
        return status is null ? null : new GetProjectStatusByIdResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId);
    }

    public async Task<AddProjectStatusResponse> AddAsync(AddProjectStatusCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var status = new ProjectStatusModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            Name = command.Name,
            Color = command.Color,
            Order = command.Order,
            IsFinal = command.IsFinal,
            CompanyId = companyId
        };

        var created = await repository.InsertAsync(status, cancellationToken);
        return new AddProjectStatusResponse(created.Id, created.ProjectId, created.Name, created.Color, created.Order, created.IsFinal, created.CompanyId);
    }

    public async Task<UpdateProjectStatusResponse?> UpdateAsync(UpdateProjectStatusCommand command, Guid companyId, CancellationToken cancellationToken = default)
    {
        var status = new ProjectStatusModel
        {
            Id = command.Id,
            ProjectId = command.ProjectId,
            Name = command.Name,
            Color = command.Color,
            Order = command.Order,
            IsFinal = command.IsFinal,
            CompanyId = companyId
        };

        var updated = await repository.UpdateAsync(command.Id, status, cancellationToken);
        return updated is null ? null : new UpdateProjectStatusResponse(updated.Id, updated.ProjectId, updated.Name, updated.Color, updated.Order, updated.IsFinal, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectStatusCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}
