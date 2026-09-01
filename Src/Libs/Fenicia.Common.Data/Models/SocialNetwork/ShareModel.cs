using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("shares", Schema = "social_network")]
public class ShareModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid OriginalFeedId { get; set; }

    [MaxLength(512)]
    public string? Text { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    [ForeignKey(nameof(OriginalFeedId))]
    public FeedModel OriginalFeed { get; set; } = null!;

    public DateTime ShareDate { get; set; } = DateTime.UtcNow;
}
