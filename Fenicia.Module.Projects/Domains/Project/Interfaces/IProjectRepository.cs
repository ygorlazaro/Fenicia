using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.Project.Interfaces;

public interface IProjectRepository
{
    IQueryable<ProjectModel> Query();

    Task<ProjectModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectModel> InsertAsync(ProjectModel model, CancellationToken cancellationToken = default);

    Task<ProjectModel?> UpdateAsync(Guid id, ProjectModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}