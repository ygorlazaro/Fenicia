using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.Team;

public interface ITeamRepository : IRepository<TeamModel>
{
    new IQueryable<TeamModel> Query();
}