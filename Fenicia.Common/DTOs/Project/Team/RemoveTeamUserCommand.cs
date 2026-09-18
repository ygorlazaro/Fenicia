using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.Team;

public class RemoveTeamUserCommand()
{
    public RemoveTeamUserCommand([Required] Guid teamId, [Required] Guid userId)
        : this()
    {
        TeamId = teamId;
        UserId = userId;
    }

    [Required]
    public Guid TeamId { get; set; }

    [Required]
    public Guid UserId { get; set; }
}
