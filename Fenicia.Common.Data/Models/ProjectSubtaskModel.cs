using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("project_subtasks", Schema = "project")]
public class ProjectSubtaskModel : BaseCompanyModel
{
    public Guid TaskId { get; set; }

    public string Title { get; set; } = null!;

    public bool IsCompleted { get; set; } = false;

    public int Order { get; set; } = 0;

    public DateTime? CompletedAt { get; set; }

    public virtual ProjectTaskModel TaskModel { get; set; } = null!;

    public DateTime? DueDate { get; set; }
}
