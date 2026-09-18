using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Project.ProjectComment;

public class UpdateProjectCommentCommand()
{
    public UpdateProjectCommentCommand([Required] Guid id, [Required] [MaxLength(200)] string content)
        : this()
    {
        Id = id;
        Content = content;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Content { get; set; } = string.Empty;
}
