using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class AddFriendshipResponse()
{
    [Required] public Guid Id { get; init; }
    [Required] public Guid ProfileId { get; init; }
    [Required] public Guid TargetProfileId { get; init; }
    [Required] public DateTime FollowDate { get; init; }
    public bool IsActive { get; init; }

    public AddFriendshipResponse(Guid Id, Guid ProfileId, Guid TargetProfileId, DateTime FollowDate, bool IsActive)
        : this()
    {
        this.Id = Id;
        this.ProfileId = ProfileId;
        this.TargetProfileId = TargetProfileId;
        this.FollowDate = FollowDate;
        this.IsActive = IsActive;
    }
}
