using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.Project;

[Table("projects", Schema = "project")]
public sealed class ProjectModel : BaseCompanyModel
{
    [MaxLength(256)]
    public string Title { get; init; } = string.Empty;

    [MaxLength(4096)]
    public string? Description { get; init; } = null;

    public EnumProjectStatus Status { get; init; } = EnumProjectStatus.Active;

    public DateTime? StartDate { get; init; } = null;

    public DateTime? EndDate { get; init; } = null;

    public Guid Owner { get; init; }

    public List<ProjectStatusModel> Statuses { get; init; } = [];

    public List<ProjectTaskModel> Tasks { get; init; } = [];

    public List<SprintModel> Sprints { get; init; } = [];

    public List<TeamModel> Teams { get; init; } = [];
}