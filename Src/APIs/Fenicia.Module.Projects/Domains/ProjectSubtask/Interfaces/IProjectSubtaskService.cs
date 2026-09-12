using Fenicia.Common.DTOs.Project.ProjectSubtask;

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;

public interface IProjectSubtaskService
{
    Task<List<GetAllProjectSubtaskResponse>> GetAllAsync(
        GetAllProjectSubtaskQuery query,
        CancellationToken cancellationToken = default);

    Task<GetProjectSubtaskByIdResponse?> GetByIdAsync(
        GetProjectSubtaskByIdQuery query,
        CancellationToken cancellationToken = default);

    Task<AddProjectSubtaskResponse> AddAsync(
        AddProjectSubtaskCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task<UpdateProjectSubtaskResponse?> UpdateAsync(
        UpdateProjectSubtaskCommand command,
        Guid companyId,
        CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteProjectSubtaskCommand command, CancellationToken cancellationToken = default);
}