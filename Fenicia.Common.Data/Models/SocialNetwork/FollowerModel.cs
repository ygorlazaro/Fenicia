using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Fenicia.Common.Data.Models.SocialNetwork;

public class FollowerModel : BaseModel
{
    [Required]
    public Guid UserId { get; set; }

    [Required]
    public Guid FollowerId { get; set; }

    [Required]
    public DateTime FollowDate { get; set; }

    [Required]
    public bool IsActive
    {
        get;
        set;
    }
= true;

    [ForeignKey(nameof(UserId))]
    public UserModel User { get; set; }

    [ForeignKey(nameof(FollowerId))]
    public UserModel Follower { get; set; }
}