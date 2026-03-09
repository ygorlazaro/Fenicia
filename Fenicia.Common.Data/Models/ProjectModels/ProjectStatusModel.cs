using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.ProjectModels;

[Table("statuses", Schema = "project")]
public class ProjectStatusModel : BaseCompanyModel
{
    public Guid ProjectId { get; set; }

    [MaxLength(30)]
    public string Name { get; set; } = null!;

    [MaxLength(7)]
    public string Color { get; set; } = null!;

    public int Order { get; set; } = 0;

    public bool IsFinal { get; set; } = false;

    public virtual ProjectModel ProjectModel { get; set; } = null!;

    public virtual List<ProjectTaskModel> Tasks { get; set; } = [];
}
