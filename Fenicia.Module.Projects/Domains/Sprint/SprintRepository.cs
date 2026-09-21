using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.Sprint.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Sprint;

public class SprintRepository(DbContext context) : Repository<SprintModel>(context), ISprintRepository
{
    public new IQueryable<SprintModel> Query()
    {
        return base.Query();
    }

    public new Task<SprintModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public new Task<SprintModel> InsertAsync(SprintModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<SprintModel?> UpdateAsync(Guid id, SprintModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<SprintModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}