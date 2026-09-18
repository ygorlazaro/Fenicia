using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectSubtask;

public class AddProjectSubtaskCommand()
{
    public AddProjectSubtaskCommand(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] [MaxLength(200)] string title,
        bool isCompleted,
        int order,
        DateTime? completedAt)
        : this()
    {
        Id = id;
        TaskId = taskId;
        Title = title;
        IsCompleted = isCompleted;
        Order = order;
        CompletedAt = completedAt;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int Order { get; set; }

    public DateTime? CompletedAt { get; set; }
}
