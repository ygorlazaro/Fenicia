using Fenicia.Common.DTOs.Project.Team;

namespace Fenicia.Module.Projects.Domains.Team.Interfaces;

public interface ITeamService
{
    Task<List<TeamResponse>> GetAllByProjectAsync(Guid projectId, GetAllTeamQuery query, CancellationToken cancellationToken = default);

    Task<TeamResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<TeamResponse> AddAsync(TeamRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task<TeamResponse?> UpdateAsync(TeamRequest command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(DeleteTeamCommand command, CancellationToken cancellationToken = default);

    Task<AddTeamUserResponse> AddMemberAsync(AddTeamUserCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task RemoveMemberAsync(RemoveTeamUserCommand command, CancellationToken cancellationToken = default);

    Task<bool> UpdateMemberRoleAsync(UpdateTeamUserRoleCommand command, CancellationToken cancellationToken = default);

    Task<bool> IsTeamAdminAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default);

    Task<bool> IsProjectAdminAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
}