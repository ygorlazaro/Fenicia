using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTaskAssignee;

public class UpdateProjectTaskAssigneeResponse()
{
    public UpdateProjectTaskAssigneeResponse(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] Guid userId,
        [Required] [MaxLength(200)] string role,
        [Required] DateTime assignedAt,
        [Required] Guid companyId)
        : this()
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;
        Role = role;
        AssignedAt = assignedAt;
        CompanyId = companyId;
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

    [Required]
    public Guid CompanyId { get; set; }
}
