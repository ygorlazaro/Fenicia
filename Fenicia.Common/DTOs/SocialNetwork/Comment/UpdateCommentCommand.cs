using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class UpdateCommentCommand()
{
    public UpdateCommentCommand(Guid id, string text)
        : this()
    {
        Id = id;
        Text = text;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Text { get; set; } = string.Empty;
}
