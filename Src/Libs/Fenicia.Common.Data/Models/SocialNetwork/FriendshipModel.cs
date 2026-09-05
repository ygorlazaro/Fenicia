using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

[Table("friendships", Schema = "social_network")]
public class FriendshipModel : BaseModel
{
    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid TargetProfileId { get; init; }

    [ForeignKey(nameof(ProfileId))]
    public ProfileModel Profile { get; init; } = default!;

    [ForeignKey(nameof(TargetProfileId))]
    public ProfileModel TargetProfile { get; init; } = default!;

    public DateTime FollowDate { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;
}
