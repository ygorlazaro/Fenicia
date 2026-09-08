using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("likes", Schema = "social_network")]
public class LikeModel : BaseCompanyModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    public Guid? CommentId { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; init; } = default!;

    [ForeignKey(nameof(CommentId))]
    public CommentModel? Comment { get; init; }

    public DateTime LikeDate { get; init; } = DateTime.UtcNow;
}
