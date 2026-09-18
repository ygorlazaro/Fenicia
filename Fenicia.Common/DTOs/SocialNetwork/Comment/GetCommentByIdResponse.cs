using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class GetCommentByIdResponse()
{
    public GetCommentByIdResponse(
        Guid id,
        Guid profileId,
        Guid feedId,
        Guid? parentCommentId,
        string text,
        DateTime commentDate,
        DateTime? updatedDate)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        ParentCommentId = parentCommentId;
        Text = text;
        CommentDate = commentDate;
        UpdatedDate = updatedDate;
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

    public DateTime? UpdatedDate { get; init; }
}
