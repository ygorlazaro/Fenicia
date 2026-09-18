using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class FollowCommand()
{
    [Required] public Guid TargetProfileId { get; set; }

    public FollowCommand(Guid TargetProfileId)
        : this()
    {
        this.TargetProfileId = TargetProfileId;
    }
}
