using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class GetFollowingResponse()
{
    [Required] public Guid Id { get; init; }
    [Required] public Guid TargetProfileId { get; init; }
    [Required] public DateTime FollowDate { get; init; }

    public GetFollowingResponse(Guid Id, Guid TargetProfileId, DateTime FollowDate)
        : this()
    {
        this.Id = Id;
        this.TargetProfileId = TargetProfileId;
        this.FollowDate = FollowDate;
    }
}
