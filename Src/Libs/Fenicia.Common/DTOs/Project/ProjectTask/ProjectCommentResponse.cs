using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public record ProjectCommentResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Content,
    [Required] Guid AuthorId);