using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class UnfollowCommand()
{
    [Required] public Guid TargetProfileId { get; set; }

    public UnfollowCommand(Guid TargetProfileId)
        : this()
    {
        this.TargetProfileId = TargetProfileId;
    }
}
