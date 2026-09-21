using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;

public interface IProjectTaskRepository
{
    IQueryable<ProjectTaskModel> Query();

    Task<ProjectTaskModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectTaskModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectTaskModel> InsertAsync(ProjectTaskModel model, CancellationToken cancellationToken = default);

    Task<ProjectTaskModel?> UpdateAsync(Guid id, ProjectTaskModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectTaskModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}