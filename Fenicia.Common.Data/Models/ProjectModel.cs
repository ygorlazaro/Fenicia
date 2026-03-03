using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("projects", Schema = "project")]
public class Project : BaseCompanyModel
{
    public string Title { get; set; } = null!;

    public string? Description { get; set; } = null;

    public Enums.Project.ProjectStatus Status { get; set; } = Enums.Project.ProjectStatus.Active;

    public DateTime? StartDate { get; set; } = null;

    public DateTime? EndDate { get; set; } = null;

    public Guid Owner { get; set; }

    public virtual List<ProjectStatusModel> Statuses { get; set; } = [];

    public virtual List<ProjectTaskModel> Tasks { get; set; } = [];
}
