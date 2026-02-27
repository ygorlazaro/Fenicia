using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models;

[Table("followers", Schema = "social_network")]
public class SNFollower : BaseModel
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
    public AuthUser User { get; set; } = null!;

    [ForeignKey(nameof(FollowerId))]
    public AuthUser Follower { get; set; } = null!;
}
