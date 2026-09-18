using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectTask;

public class ProjectCommentResponse()
{
    public ProjectCommentResponse(
        [Required] Guid id,
        [Required] [MaxLength(200)] string content,
        [Required] Guid authorId)
        : this()
    {
        Id = id;
        Content = content;
        AuthorId = authorId;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Content { get; set; } = string.Empty;

    [Required]
    public Guid AuthorId { get; set; }
}
