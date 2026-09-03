using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("friendships", Schema = "social_network")]
public class FriendshipModel : BaseModel
{
    [Required]
    public Guid UserId { get; init; }

    [Required]
    public Guid TargetUserId { get; init; }

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; init; } = default!;

    [ForeignKey(nameof(TargetUserId))]
    public UserModel TargetUser { get; init; } = default!;

    public DateTime FollowDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}