using Fenicia.Module.Projects.Domains.Team.DTOs;

namespace Fenicia.Module.Projects.Domains.Team.Interfaces;

public interface ITeamService
{
    Task<List<GetAllTeamResponse>> GetAllByProjectAsync(Guid projectId, CancellationToken cancellationToken = default);

    Task<GetTeamByIdResponse?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<List<TeamMemberResponse>> GetMembersAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<AddTeamResponse> AddAsync(AddTeamCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task<UpdateTeamResponse?> UpdateAsync(UpdateTeamCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    Task<AddTeamUserResponse> AddMemberAsync(AddTeamUserCommand command, Guid companyId, CancellationToken cancellationToken = default);

    Task RemoveMemberAsync(RemoveTeamUserCommand command, CancellationToken cancellationToken = default);

    Task<bool> UpdateMemberRoleAsync(UpdateTeamUserRoleCommand command, CancellationToken cancellationToken = default);

    Task<bool> IsTeamAdminAsync(Guid userId, Guid teamId, CancellationToken cancellationToken = default);

    Task<bool> IsProjectAdminAsync(Guid userId, Guid projectId, CancellationToken cancellationToken = default);
}
