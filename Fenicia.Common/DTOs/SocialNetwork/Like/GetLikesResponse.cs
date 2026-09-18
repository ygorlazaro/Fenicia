using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class GetLikesResponse()
{
    public GetLikesResponse(
        Guid id,
        Guid profileId,
        Guid feedId,
        DateTime likeDate)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        LikeDate = likeDate;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    [Required]
    public DateTime LikeDate { get; init; }
}
