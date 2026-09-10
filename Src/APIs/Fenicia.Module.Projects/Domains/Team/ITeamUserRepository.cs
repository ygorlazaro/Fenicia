using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Module.Projects.Domains.Team;

public interface ITeamUserRepository : IRepository<TeamUserModel>
{
    new IQueryable<TeamUserModel> Query();

    Task<TeamUserModel?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken ct = default);

    Task<List<TeamUserModel>> GetByTeamAsync(Guid teamId, CancellationToken ct = default);

    Task<List<TeamUserModel>> GetByUserAsync(Guid userId, CancellationToken ct = default);
}