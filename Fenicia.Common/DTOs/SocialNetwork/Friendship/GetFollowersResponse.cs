using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public class GetFollowersResponse()
{
    [Required] public Guid Id { get; init; }
    [Required] public Guid ProfileId { get; init; }
    [Required] public DateTime FollowDate { get; init; }

    public GetFollowersResponse(Guid Id, Guid ProfileId, DateTime FollowDate)
        : this()
    {
        this.Id = Id;
        this.ProfileId = ProfileId;
        this.FollowDate = FollowDate;
    }
}
