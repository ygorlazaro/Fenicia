using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.Project;

[Table("project_subtasks", Schema = "project")]
public sealed class ProjectSubtaskModel : BaseCompanyModel
{
    public Guid TaskId { get; init; }

    [MaxLength(256)]
    public string Title { get; init; } = string.Empty;

    public bool IsCompleted { get; init; } = false;

    public int Order { get; init; } = 0;

    public DateTime? CompletedAt { get; init; }

    public ProjectTaskModel TaskModel { get; init; } = default!;

    public DateTime? DueDate { get; init; }
}