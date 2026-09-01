using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;

public interface IProjectStatusRepository : IRepository<ProjectStatusModel>
{
    Task<IEnumerable<ProjectStatusModel>> GetAllByCompanyAsync(Guid companyId, int page = 1, int perPage = 10, CancellationToken cancellationToken = default);

    Task<ProjectStatusModel?> GetByIdAndCompanyAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);
}