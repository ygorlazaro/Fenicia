using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("likes", Schema = "social_network")]
public class LikeModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid FeedId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    [ForeignKey(nameof(FeedId))]
    public FeedModel Feed { get; set; } = null!;

    public DateTime LikeDate { get; set; } = DateTime.UtcNow;
}
