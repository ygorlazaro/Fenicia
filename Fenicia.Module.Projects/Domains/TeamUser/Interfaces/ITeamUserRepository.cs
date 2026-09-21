using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.TeamUser.Interfaces;

public interface ITeamUserRepository
{
    IQueryable<TeamUserModel> Query();

    Task<TeamUserModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeamUserModel?> GetByTeamAndUserAsync(Guid teamId, Guid userId, CancellationToken cancellationToken = default);

    Task<List<TeamUserModel>> GetByTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<List<TeamUserModel>> GetByUserAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<TeamUserModel> InsertAsync(TeamUserModel model, CancellationToken cancellationToken = default);

    Task<TeamUserModel?> UpdateAsync(Guid id, TeamUserModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TeamUserModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}