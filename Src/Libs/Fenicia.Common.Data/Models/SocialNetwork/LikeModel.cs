using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("likes", Schema = "social_network")]
public class LikeModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; init; } = default!;

    public DateTime LikeDate { get; init; } = DateTime.UtcNow;
}