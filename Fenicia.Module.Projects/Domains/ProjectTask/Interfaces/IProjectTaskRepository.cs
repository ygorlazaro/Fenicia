using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.ProjectTask.Interfaces;

public interface IProjectTaskRepository : IRepository<ProjectTaskModel>
{
    Task<ProjectTaskModel?> GetByIdWithRelationsAsync(Guid id, CancellationToken cancellationToken = default);
}