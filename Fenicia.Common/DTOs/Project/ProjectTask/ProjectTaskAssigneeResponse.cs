using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class ProjectTaskAssigneeResponse()
{
    public ProjectTaskAssigneeResponse(
        [Required] Guid id,
        [Required] Guid userId,
        [Required] [MaxLength(200)] string userName,
        [Required] [MaxLength(200)] string userEmail)
        : this()
    {
        Id = id;
        UserId = userId;
        UserName = userName;
        UserEmail = userEmail;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(200)]
    public string UserName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string UserEmail { get; set; } = string.Empty;
}
