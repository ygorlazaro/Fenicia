using Fenicia.Module.Projects.Domains.ProjectStatus.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;

public interface IProjectStatusService
{
    Task<List<GetAllProjectStatusResponse>> GetAllAsync(
        GetAllProjectStatusQuery query,
        CancellationToken cancellationToken = default);

    Task<GetProjectStatusByIdResponse?> GetByIdAsync(
        GetProjectStatusByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProjectStatusResponse> AddAsync(
        AddProjectStatusCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProjectStatusResponse?> UpdateAsync(
        UpdateProjectStatusCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectStatusCommand command, CancellationToken cancellationToken = default);
}