using Fenicia.Common.Data.Models.Project;

namespace Fenicia.Module.Projects.Domains.Team.Interfaces;

public interface ITeamRepository
{
    IQueryable<TeamModel> Query();

    Task<TeamModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeamModel> InsertAsync(TeamModel model, CancellationToken cancellationToken = default);

    Task<TeamModel?> UpdateAsync(Guid id, TeamModel model, CancellationToken cancellationToken = default);

    Task<int> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<TeamModel>> GetAllAsync(int page, int perPage, CancellationToken cancellationToken = default);
}