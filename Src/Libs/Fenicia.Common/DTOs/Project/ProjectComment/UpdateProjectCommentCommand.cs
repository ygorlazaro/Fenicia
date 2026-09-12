using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public record UpdateProjectCommentCommand([Required] Guid Id, [Required] [MaxLength(200)] string Content);