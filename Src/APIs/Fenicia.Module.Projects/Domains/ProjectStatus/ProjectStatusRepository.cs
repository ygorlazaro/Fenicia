using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.ProjectStatus.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.ProjectStatus;

public class ProjectStatusRepository(DefaultContext context)
    : Repository<ProjectStatusModel>(context), IProjectStatusRepository
{
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

    public Task<ProjectStatusModel?> GetByIdAndCompanyAsync(
        Guid id,
        Guid companyId,
        CancellationToken cancellationToken = default)
    {
        return DbSet.FirstOrDefaultAsync(e => e.Id == id && e.CompanyId == companyId, cancellationToken);
    }
}