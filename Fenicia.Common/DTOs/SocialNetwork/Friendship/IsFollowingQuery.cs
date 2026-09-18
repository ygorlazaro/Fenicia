using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class IsFollowingQuery()
{
    [Required] public Guid TargetProfileId { get; set; }

    public IsFollowingQuery(Guid TargetProfileId)
        : this()
    {
        this.TargetProfileId = TargetProfileId;
    }
}
