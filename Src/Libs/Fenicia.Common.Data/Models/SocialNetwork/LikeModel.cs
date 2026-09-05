using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("likes", Schema = "social_network")]
public class LikeModel : BaseCompanyModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; init; } = default!;

    public DateTime LikeDate { get; init; } = DateTime.UtcNow;
}
