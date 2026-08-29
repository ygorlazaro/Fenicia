using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Enums.Project;

namespace Fenicia.Common.Data.Models.ProjectModels;

[Table("projects", Schema = "project")]
public class ProjectModel : BaseCompanyModel
{
    [MaxLength(256)]
    public string Title { get; set; } = null!;

    [MaxLength(4096)]
    public string? Description { get; set; } = null;

    public EnumProjectStatus Status { get; set; } = EnumProjectStatus.Active;

    public DateTime? StartDate { get; set; } = null;

    public DateTime? EndDate { get; set; } = null;

    public Guid Owner { get; set; }

    public virtual List<ProjectStatusModel> Statuses { get; set; } = [];

    public virtual List<ProjectTaskModel> Tasks { get; set; } = [];
}