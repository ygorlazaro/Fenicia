using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

[Table("team_users", Schema = "project")]
public sealed class TeamUserModel : BaseCompanyModel
{
    [Required]
    public Guid TeamId { get; init; }

    [Required]
    public Guid UserId { get; init; }

    [Required]
    public EnumTeamRole Role { get; set; } = EnumTeamRole.User;

    public DateTime JoinedAt { get; init; } = DateTime.UtcNow;

    [ForeignKey(nameof(TeamId))]
    public TeamModel Team { get; init; } = default!;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;
}
