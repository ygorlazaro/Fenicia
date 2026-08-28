using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;
using Fenicia.Module.Projects.Domains.ProjectStatus;
using Fenicia.Common.Data.Models.ProjectModels;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusService(ProjectStatusRepository repository)
{
    public async Task<List<GetAllProjectStatusResponse>> GetAllAsync(GetAllProjectStatusQuery query, CancellationToken ct)
    {
        var statuses = await repository.GetAllAsync(query.Page, query.PerPage, ct);
        return statuses.Select(s => new GetAllProjectStatusResponse(s.Id, s.ProjectId, s.Name, s.Color, s.Order, s.IsFinal, s.CompanyId)).ToList();
    }

    public async Task<GetProjectStatusByIdResponse?> GetByIdAsync(GetProjectStatusByIdQuery query, CancellationToken ct)
    {
        var status = await repository.GetByIdAsync(query.Id, ct);
        return status is null ? null : new GetProjectStatusByIdResponse(status.Id, status.ProjectId, status.Name, status.Color, status.Order, status.IsFinal, status.CompanyId);
    }

    public async Task<AddProjectStatusResponse> AddAsync(AddProjectStatusCommand command, Guid companyId, CancellationToken ct)
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

        var created = await repository.InsertAsync(status, ct);
        return new AddProjectStatusResponse(created.Id, created.ProjectId, created.Name, created.Color, created.Order, created.IsFinal, created.CompanyId);
    }

    public async Task<UpdateProjectStatusResponse?> UpdateAsync(UpdateProjectStatusCommand command, Guid companyId, CancellationToken ct)
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

        var updated = await repository.UpdateAsync(command.Id, status, ct);
        return updated is null ? null : new UpdateProjectStatusResponse(updated.Id, updated.ProjectId, updated.Name, updated.Color, updated.Order, updated.IsFinal, updated.CompanyId);
    }

    public async Task DeleteAsync(DeleteProjectStatusCommand command, CancellationToken ct)
    {
        await repository.DeleteAsync(command.Id, ct);
    }
}
