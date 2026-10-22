using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common;

/// <summary>
/// Fenicia.Module.Projects/Domains/ProjectStatus/ProjectStatusRepository.cs implementation.
/// </summary>

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusRepository(DbContext context)
    : Repository<ProjectStatusModel>(context), IProjectStatusRepository
{
    public override IQueryable<ProjectStatusModel> Query()
    {
        return base.Query();
    }

    public override Task<ProjectStatusModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public override Task<ProjectStatusModel?> GetByIdAndCompanyAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId, cancellationToken);
    }

    public override Task<ProjectStatusModel> InsertAsync(ProjectStatusModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public override Task<ProjectStatusModel?> UpdateAsync(Guid id, ProjectStatusModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public override Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public override Task<IEnumerable<ProjectStatusModel>> GetAllAsync(PaginationQuery query, CancellationToken cancellationToken = default)
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
            .Skip((query.Page - 1) * query.PerPage)
            .Take(query.PerPage)
            .ToListAsync(cancellationToken);
    }
}