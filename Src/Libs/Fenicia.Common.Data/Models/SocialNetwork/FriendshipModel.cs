using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("friendships", Schema = "social_network")]
public class FriendshipModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid TargetUserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; } = null!;

    [ForeignKey(nameof(TargetUserId))]
    public UserModel TargetUser { get; set; } = null!;

    public DateTime FollowDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
