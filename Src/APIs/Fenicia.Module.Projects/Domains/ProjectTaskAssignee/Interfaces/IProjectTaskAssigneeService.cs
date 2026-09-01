using Fenicia.Module.Projects.Domains.ProjectTaskAssignee.DTOs;

namespace Fenicia.Module.Projects.Domains.ProjectTaskAssignee.Interfaces;

public interface IProjectTaskAssigneeService
{
    Task<List<GetAllProjectTaskAssigneeResponse>> GetAllAsync(GetAllProjectTaskAssigneeQuery query, CancellationToken cancellationToken = default);

    Task<GetProjectTaskAssigneeByIdResponse?> GetByIdAsync(GetProjectTaskAssigneeByIdQuery query, CancellationToken cancellationToken = default);

    Task<AddProjectTaskAssigneeResponse> AddAsync(AddProjectTaskAssigneeCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateProjectTaskAssigneeResponse?> UpdateAsync(UpdateProjectTaskAssigneeCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectTaskAssigneeCommand command, CancellationToken cancellationToken = default);
}