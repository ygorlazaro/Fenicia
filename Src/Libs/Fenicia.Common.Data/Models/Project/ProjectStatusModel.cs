using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Project;

[Table("statuses", Schema = "project")]
public sealed class ProjectStatusModel : BaseCompanyModel
{
    public Guid ProjectId { get; init; }

    [MaxLength(30)]
    public string Name { get; init; } = string.Empty;

    [MaxLength(7)]
    public string Color { get; init; } = string.Empty;

    public int Order { get; init; } = 0;

    public bool IsFinal { get; init; } = false;

    [ForeignKey(nameof(ProjectId))]
    public ProjectModel ProjectModel { get; init; } = default!;

    public List<ProjectTaskModel> Tasks { get; init; } = [];
}