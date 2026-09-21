using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Fenicia.Module.Projects.Domains.TeamUser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.TeamUser;

public class TeamUserRepository(DefaultContext context) : Repository<TeamUserModel>(context), ITeamUserRepository
{
    public new IQueryable<TeamUserModel> Query()
    {
        return DbSet
            .Include(tu => tu.User)
            .Include(tu => tu.Team);
    }

    public new Task<TeamUserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.GetByIdAsync(id, cancellationToken);
    }

    public Task<TeamUserModel?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(tu => tu.User)
            .FirstOrDefaultAsync(tu => tu.TeamId == teamId && tu.UserId == userId, cancellationToken);
    }

    public Task<List<TeamUserModel>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(tu => tu.User)
            .Where(tu => tu.TeamId == teamId)
            .ToListAsync(cancellationToken);
    }

    public Task<List<TeamUserModel>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return DbSet
            .Include(tu => tu.Team)
            .Where(tu => tu.UserId == userId)
            .ToListAsync(cancellationToken);
    }

    public new Task<TeamUserModel> InsertAsync(TeamUserModel model, CancellationToken cancellationToken = default)
    {
        return base.InsertAsync(model, cancellationToken);
    }

    public new Task<TeamUserModel?> UpdateAsync(Guid id, TeamUserModel model, CancellationToken cancellationToken = default)
    {
        return base.UpdateAsync(id, model, cancellationToken);
    }

    public new Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return base.DeleteAsync(id, cancellationToken);
    }

    public new Task<IEnumerable<TeamUserModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default)
    {
        return base.GetAllAsync(page, perPage, cancellationToken);
    }
}