using Fenicia.Common.Data.Models.Project;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectSubtask/Interfaces/IProjectSubtaskRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectSubtask.Interfaces;

public interface IProjectSubtaskRepository
{
    IQueryable<ProjectSubtaskModel> Query();

    Task<ProjectSubtaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectSubtaskModel> InsertAsync(ProjectSubtaskModel model, CancellationToken cancellationToken = default);

    Task<ProjectSubtaskModel?> UpdateAsync(Guid id, ProjectSubtaskModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectSubtaskModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default);
}