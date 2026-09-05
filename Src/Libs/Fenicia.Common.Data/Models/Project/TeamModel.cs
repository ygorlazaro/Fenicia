using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Project;

[Table("teams", Schema = "project")]
public sealed class TeamModel : BaseCompanyModel
{
    [Required]
    [MaxLength(128)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; } = null;

    [MaxLength(30)]
    public string Color { get; set; } = "#6366f1";

    [Required]
    public Guid ProjectId { get; init; }

    [Required]
    public Guid CreatedBy { get; init; }

    [ForeignKey(nameof(CreatedBy))]
    public UserModel Creator { get; init; } = default!;

    [ForeignKey(nameof(ProjectId))]
    public ProjectModel Project { get; init; } = default!;

    public List<TeamUserModel> Members { get; init; } = [];
}
