using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Fenicia.Common.Data.Models.Auth;

namespace Fenicia.Common.Data.Models.SocialNetworkModels;

[Table("followers", Schema = "social_network")]
public class FollowerModel : BaseCompanyModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid FollowerId { get; set; }

    [Required]
    public DateTime FollowDate { get; set; }

    [Required]
    public bool IsActive { get; set; }
        = true;

    [ForeignKey(nameof(UserId))]
    public UserModel UserModel { get; set; } = null!;

    [ForeignKey(nameof(FollowerId))]
    public UserModel Follower { get; set; } = null!;
}
