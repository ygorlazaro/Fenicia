using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.Projects.Domains.ProjectTask.DTOs;

public record ProjectCommentResponse([Required] Guid Id, [Required][MaxLength(200)] string Content, [Required] Guid AuthorId);