using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectComment.DTOs;

public record UpdateProjectCommentCommand([Required] Guid Id, [Required][MaxLength(200)] string Content);