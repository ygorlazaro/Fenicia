using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record AddProjectCommentCommand(
    [Required] Guid Id,
    [Required] Guid TaskId,
    [Required] Guid UserId,
    [Required] [MaxLength(4096)] string Content,
    string? UserName = null);