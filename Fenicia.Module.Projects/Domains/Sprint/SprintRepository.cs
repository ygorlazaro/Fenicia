using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/Sprint/SprintRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintRepository(DbContext context) : Repository<SprintModel>(context), ISprintRepository
{
    public IQueryable<SprintModel> Query()
    {
        return base.Query();
    }

    public Task<SprintModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<SprintModel> InsertAsync(SprintModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public Task<SprintModel?> UpdateAsync(Guid id, SprintModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public Task<IEnumerable<SprintModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}