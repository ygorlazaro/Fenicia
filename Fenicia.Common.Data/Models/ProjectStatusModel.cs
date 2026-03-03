using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("statuses", Schema = "project")]
public class ProjectStatusModel : BaseCompanyModel
{
    public Guid ProjectId { get; set; }

    public string Name { get; set; } = null!;

    public string Color { get; set; } = null!;

    public int Order { get; set; } = 0;

    public bool IsFinal { get; set; } = false;

    public virtual ProjectModel ProjectModel { get; set; } = null!;

    public virtual List<ProjectTaskModel> Tasks { get; set; } = [];
}
