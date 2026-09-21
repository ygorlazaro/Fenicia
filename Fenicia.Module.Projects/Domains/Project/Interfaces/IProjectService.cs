using Fenicia.Common.DTOs.Project.Project;

namespace Fenicia.Module.Projects.Domains.Project.Interfaces;

public interface IProjectService
{
    Task<List<ProjectResponse>> GetAllAsync(GetAllProjectQuery query, CancellationToken cancellationToken = default);

    Task<ProjectResponse?> GetByIdAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default);

    Task<ProjectResponse> AddAsync(ProjectRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task<ProjectResponse?> UpdateAsync(ProjectRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default);
}