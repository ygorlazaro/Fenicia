using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("shares", Schema = "social_network")]
public class ShareModel : BaseCompanyModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid OriginalFeedId { get; init; }

    [MaxLength(512)]
    public string? Text { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(OriginalFeedId))]
    public FeedModel OriginalFeed { get; init; } = default!;

    public DateTime ShareDate { get; init; } = DateTime.UtcNow;
}
