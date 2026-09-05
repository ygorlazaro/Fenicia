using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Team;

public interface ITeamUserRepository : IRepository<TeamUserModel>
{
    new IQueryable<TeamUserModel> Query();

    Task<TeamUserModel?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken ct = default);

    Task<List<TeamUserModel>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);

    Task<List<TeamUserModel>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}

public class TeamUserRepository(DefaultContext context) : Repository<TeamUserModel>(context), ITeamUserRepository
{
    public new IQueryable<TeamUserModel> Query() => DbSet
        .Include(tu => tu.User)
        .Include(tu => tu.Team);

    public Task<TeamUserModel?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken ct = default)
    {
        return DbSet
            .Include(tu => tu.User)
            .FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId, ct);
    }

    public Task<List<TeamUserModel>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
    {
        return DbSet
            .Include(tu => tu.User)
            .Where(tu => tu.TeamId == teamId)
            .ToListAsync(ct);
    }

    public Task<List<TeamUserModel>> GetByUserAsync(Guid userId, CancellationToken ct = default)
    {
        return DbSet
            .Include(tu => tu.Team)
            .Where(tu => tu.UserId == userId)
            .ToListAsync(ct);
    }
}
