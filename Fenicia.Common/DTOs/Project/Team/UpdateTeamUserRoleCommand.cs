using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class UpdateTeamUserRoleCommand()
{
    public UpdateTeamUserRoleCommand(
        [Required] Guid teamId,
        [Required] Guid userId,
        [Required] string role)
        : this()
    {
        TeamId = teamId;
        UserId = userId;
        Role = role;
    }

    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;
}
