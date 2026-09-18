using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class AddCommentCommand()
{
    public AddCommentCommand(
        Guid id,
        Guid profileId,
        Guid feedId,
        Guid? parentCommentId,
        string text)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        ParentCommentId = parentCommentId;
        Text = text;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    public Guid ProfileId { get; set; }

    [Required]
    public Guid FeedId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Text { get; set; } = string.Empty;
}
