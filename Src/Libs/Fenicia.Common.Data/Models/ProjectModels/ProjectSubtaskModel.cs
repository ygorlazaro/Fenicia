using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.ProjectModels;

[Table("project_subtasks", Schema = "project")]
public class ProjectSubtaskModel : BaseCompanyModel
{
    public Guid TaskId { get; set; }

    [MaxLength(256)]
    public string Title { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;

    public int Order { get; set; } = 0;

    public DateTime? CompletedAt { get; set; }

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public DateTime? DueDate { get; set; }
}