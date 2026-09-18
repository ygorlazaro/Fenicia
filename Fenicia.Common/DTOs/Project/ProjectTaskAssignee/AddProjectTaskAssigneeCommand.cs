using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTaskAssignee;

public class AddProjectTaskAssigneeCommand()
{
    public AddProjectTaskAssigneeCommand(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] Guid userId,
        [Required] [MaxLength(200)] string role,
        [Required] DateTime assignedAt)
        : this()
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;
        Role = role;
        AssignedAt = assignedAt;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public DateTime AssignedAt { get; set; }
}
