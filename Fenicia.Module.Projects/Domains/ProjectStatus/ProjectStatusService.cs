using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.ProjectStatus;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusService(IProjectStatusRepository repository, ProjectStatusMapper mapper) : IProjectStatusService
{
    public async Task<List<GetAllProjectStatusResponse>> GetAllAsync(
        GetAllProjectStatusQuery query,
        CancellationToken cancellationToken = default)
    {
        var baseQuery = repository.Query();
        var statuses = await baseQuery.Skip((query.Page - 1) * query.PerPage).Take(query.PerPage)
            .ToListAsync(cancellationToken);
        return [.. statuses.Select(mapper.MapToGetAllProjectStatusResponse)];
    }

    public async Task<GetProjectStatusByIdResponse?> GetByIdAsync(
        GetProjectStatusByIdQuery query,
        CancellationToken cancellationToken = default)
    {
        var status = await repository.GetByIdAsync(query.Id, cancellationToken);
        return status is null
            ? null
            : mapper.MapToGetProjectStatusByIdResponse(status);
    }

    public async Task<AddProjectStatusResponse> AddAsync(
        AddProjectStatusCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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
        return mapper.MapToAddProjectStatusResponse(created);
    }

    public async Task<UpdateProjectStatusResponse?> UpdateAsync(
        UpdateProjectStatusCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default)
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
        return updated is null
            ? null
            : mapper.MapToUpdateProjectStatusResponse(updated);
    }

    public async Task DeleteAsync(DeleteProjectStatusCommand command, CancellationToken cancellationToken = default)
    {
        await repository.DeleteAsync(command.Id, cancellationToken);
    }
}