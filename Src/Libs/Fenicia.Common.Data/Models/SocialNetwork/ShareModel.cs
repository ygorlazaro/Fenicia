using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("shares", Schema = "social_network")]
public class ShareModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid OriginalFeedId { get; init; }

    [MaxLength(512)]
    public string? Text { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(OriginalFeedId))]
    public FeedModel OriginalFeed { get; init; } = default!;

    public DateTime ShareDate { get; init; } = DateTime.UtcNow;
}