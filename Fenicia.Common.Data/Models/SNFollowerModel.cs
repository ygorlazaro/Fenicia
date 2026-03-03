using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("followers", Schema = "social_network")]
public class SNFollowerModel : BaseCompanyModel
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
    public AuthUserModel UserModel { get; set; } = null!;

    [ForeignKey(nameof(FollowerId))]
    public AuthUserModel Follower { get; set; } = null!;
}
