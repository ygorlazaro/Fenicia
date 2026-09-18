using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class AddCommentResponse()
{
    public AddCommentResponse(
        Guid id,
        Guid profileId,
        Guid feedId,
        Guid? parentCommentId,
        string text,
        DateTime commentDate,
        Guid companyId)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        ParentCommentId = parentCommentId;
        Text = text;
        CommentDate = commentDate;
        CompanyId = companyId;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    public Guid? ParentCommentId { get; init; }

    [Required]
    [MaxLength(200)]
    public string Text { get; init; } = string.Empty;

    [Required]
    public DateTime CommentDate { get; init; }

    [Required]
    public Guid CompanyId { get; init; }
}
