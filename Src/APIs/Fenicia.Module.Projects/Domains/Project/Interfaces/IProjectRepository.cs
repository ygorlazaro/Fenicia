using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.Project.Interfaces;

public interface IProjectRepository : IRepository<ProjectModel>
{
    Task<ProjectModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}