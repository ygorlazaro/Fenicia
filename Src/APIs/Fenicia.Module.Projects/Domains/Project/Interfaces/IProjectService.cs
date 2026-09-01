using Fenicia.Module.Projects.Domains.Project.DTOs;

namespace Fenicia.Module.Projects.Domains.Project.Interfaces;

public interface IProjectService
{
    Task<List<GetAllProjectResponse>> GetAllAsync(GetAllProjectQuery query, CancellationToken cancellationToken = default);

    Task<GetProjectByIdResponse?> GetByIdAsync(GetProjectByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddProjectResponse> AddAsync(AddProjectCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateProjectResponse?> UpdateAsync(UpdateProjectCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectCommand command, CancellationToken cancellationToken = default);
}