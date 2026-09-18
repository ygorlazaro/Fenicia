using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class AddTeamUserCommand()
{
    public AddTeamUserCommand(
        [Required] Guid id,
        [Required] Guid teamId,
        [Required] Guid userId,
        [Required] string role)
        : this()
    {
        Id = id;
        TeamId = teamId;
        UserId = userId;
        Role = role;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    public string Role { get; set; } = string.Empty;
}
