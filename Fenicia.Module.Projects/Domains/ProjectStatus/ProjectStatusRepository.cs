using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusRepository(DbContext context)
    : Repository<ProjectStatusModel>(context), IProjectStatusRepository
{
    public new IQueryable<ProjectStatusModel> Query()
    {
        return base.Query();
    }

    public new Task<ProjectStatusModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<ProjectStatusModel?> GetByIdAndCompanyAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId, cancellationToken);
    }

    public new Task<ProjectStatusModel> InsertAsync(ProjectStatusModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<ProjectStatusModel?> UpdateAsync(Guid id, ProjectStatusModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<ProjectStatusModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }

    public async Task<IEnumerable<ProjectStatusModel>> GetAllByCompanyAsync(
        Guid companyId,
        int page = 1,
        int perPage = 10,
        CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(e => e.CompanyId == companyId)
            .Skip((page - 1) * perPage)
            .Take(perPage)
            .ToListAsync(cancellationToken);
    }
}