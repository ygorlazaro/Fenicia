using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Comment;

public class GetAllCommentResponse()
{
    public GetAllCommentResponse(
        Guid id,
        Guid profileId,
        Guid feedId,
        Guid? parentCommentId,
        string text,
        DateTime commentDate,
        DateTime? updatedDate,
        int totalLikes,
        int totalReplies,
        bool isMine)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        ParentCommentId = parentCommentId;
        Text = text;
        CommentDate = commentDate;
        UpdatedDate = updatedDate;
        TotalLikes = totalLikes;
        TotalReplies = totalReplies;
        IsMine = isMine;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    public Guid? ParentCommentId { get; init; }

    [Required]
    [MaxLength(1024)]
    public string Text { get; init; } = string.Empty;

    [Required]
    public DateTime CommentDate { get; init; }

    public DateTime? UpdatedDate { get; init; }

    public int TotalLikes { get; set; }

    public int TotalReplies { get; set; }

    public bool IsMine { get; set; }
}
