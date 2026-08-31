using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("comments", Schema = "social_network")]
public class CommentModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid FeedId { get; set; }

    public Guid? ParentCommentId { get; set; }

    [Required]
    [MaxLength(1024)]
    public string Text { get; set; } = string.Empty;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; set; } = null!;

    [ForeignKey(nameof(ParentCommentId))]
    public CommentModel? ParentComment { get; set; }

    public DateTime CommentDate { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedDate { get; set; }

    public List<LikeModel> Likes { get; set; } = [];

    public List<AttachmentModel> Attachments { get; set; } = [];

    public List<CommentModel> Replies { get; set; } = [];
}
