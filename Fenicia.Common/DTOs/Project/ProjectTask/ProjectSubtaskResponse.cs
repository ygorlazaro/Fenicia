using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class ProjectSubtaskResponse()
{
    public ProjectSubtaskResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string title,
        bool isCompleted,
        int order,
        DateTime? dueDate)
        : this()
    {
        Id = id;
        Title = title;
        IsCompleted = isCompleted;
        Order = order;
        DueDate = dueDate;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public bool IsCompleted { get; set; }

    public int Order { get; set; }

    public DateTime? DueDate { get; set; }
}
