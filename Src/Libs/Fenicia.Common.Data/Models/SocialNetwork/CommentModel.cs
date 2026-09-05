using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("comments", Schema = "social_network")]
public class CommentModel : BaseCompanyModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    public Guid? ParentCommentId { get; init; }

    [Required]
    [MaxLength(1024)]
    public string Text { get; init; } = string.Empty;

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; init; } = default!;

    [ForeignKey(nameof(ParentCommentId))]
    public CommentModel? ParentComment { get; init; }

    public DateTime CommentDate { get; init; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; init; }

    public List<LikeModel> Likes { get; init; } = [];

    public List<AttachmentModel> Attachments { get; init; } = [];

    public List<CommentModel> Replies { get; init; } = [];
}
