using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;

public interface IProjectStatusRepository
{
    IQueryable<ProjectStatusModel> Query();

    Task<ProjectStatusModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<ProjectStatusModel?> GetByIdAndCompanyAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    Task<ProjectStatusModel> InsertAsync(ProjectStatusModel model, CancellationToken cancellationToken = default);

    Task<ProjectStatusModel?> UpdateAsync(Guid id, ProjectStatusModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectStatusModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);

    Task<IEnumerable<ProjectStatusModel>> GetAllByCompanyAsync(Guid companyId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);
}