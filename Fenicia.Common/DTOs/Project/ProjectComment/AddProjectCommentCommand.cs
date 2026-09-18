using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public class AddProjectCommentCommand()
{
    public AddProjectCommentCommand(
        [Required] Guid id,
        [Required] Guid taskId,
        [Required] Guid userId,
        [Required] [MaxLength(4096)] string content,
        string? userName = null)
        : this()
    {
        Id = id;
        TaskId = taskId;
        UserId = userId;
        Content = content;
        UserName = userName;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid TaskId { get; set; }

    [Required]
    public Guid UserId { get; set; }

    [Required]
    [MaxLength(4096)]
    public string Content { get; set; } = string.Empty;

    public string? UserName { get; set; }
}
