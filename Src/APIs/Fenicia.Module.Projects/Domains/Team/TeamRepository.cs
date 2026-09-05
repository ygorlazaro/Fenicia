using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Projects.Domains.Team;

public interface ITeamRepository : IRepository<TeamModel>
{
    new IQueryable<TeamModel> Query();
}

public class TeamRepository(DefaultContext context) : Repository<TeamModel>(context), ITeamRepository
{
    public new IQueryable<TeamModel> Query() => DbSet
        .Include(t => t.Members)
        .ThenInclude(m => m.User);
}
