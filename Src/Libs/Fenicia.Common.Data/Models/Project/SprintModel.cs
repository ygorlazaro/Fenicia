using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.Project;

[Table("sprints", Schema = "project")]
public sealed class SprintModel : BaseCompanyModel
{
    public Guid ProjectId { get; init; }

    [MaxLength(256)]
    public string Name { get; init; } = string.Empty;

    public DateTime? StartDate { get; init; } = null;

    public DateTime? EndDate { get; init; } = null;

    [MaxLength(4096)]
    public string? Description { get; init; } = null;

    public Guid CreatedBy { get; init; } = Guid.Empty;

    [ForeignKey(nameof(ProjectId))]
    public ProjectModel ProjectModel { get; init; } = default!;

    [ForeignKey(nameof(CreatedBy))]
    public UserModel User { get; init; } = default!;

    public List<ProjectTaskModel> Tasks { get; init; } = [];
}
