using Fenicia.Common.Data.Models.Project;
using Fenicia.Common.DTOs.Project.Team;
using Riok.Mapperly.Abstractions;

namespace Fenicia.Module.Projects.Domains.Team;

[Mapper(RequiredMappingStrategy = RequiredMappingStrategy.Target)]
public partial class TeamMapper
{
    public partial GetTeamByIdResponse MapToGetTeamByIdResponse(TeamModel team);

    public partial AddTeamResponse MapToAddTeamResponse(TeamModel team);

    public partial UpdateTeamResponse MapToUpdateTeamResponse(TeamModel team);

    [MapProperty("User.Name", nameof(TeamMemberResponse.UserName))]
    [MapProperty("User.Email", nameof(TeamMemberResponse.Email))]
    public partial TeamMemberResponse MapToTeamMemberResponse(TeamUserModel teamUser);

    public partial AddTeamUserResponse MapToAddTeamUserResponse(TeamUserModel teamUser);
}
