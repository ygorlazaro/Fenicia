using Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;

public interface IProjectTaskService
{
    Task<List<GetAllProjectTaskResponse>> GetAllAsync(GetAllProjectTaskQuery query, CancellationToken cancellationToken = default);

    Task<GetProjectTaskByIdResponse?> GetByIdAsync(GetProjectTaskByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddProjectTaskResponse> AddAsync(AddProjectTaskCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateProjectTaskResponse?> UpdateAsync(UpdateProjectTaskCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectTaskCommand command, CancellationToken cancellationToken = default);
}