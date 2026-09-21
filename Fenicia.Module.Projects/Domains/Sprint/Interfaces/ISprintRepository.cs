using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.Sprint.Interfaces;

public interface ISprintRepository
{
    IQueryable<SprintModel> Query();

    Task<SprintModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<SprintModel> InsertAsync(SprintModel model, CancellationToken cancellationToken = default);

    Task<SprintModel?> UpdateAsync(Guid id, SprintModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<SprintModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}