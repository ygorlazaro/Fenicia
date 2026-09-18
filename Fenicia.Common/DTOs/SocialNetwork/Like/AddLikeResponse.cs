using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class AddLikeResponse()
{
    public AddLikeResponse(
        Guid id,
        Guid profileId,
        Guid feedId,
        DateTime likeDate,
        Guid companyId)
        : this()
    {
        Id = id;
        ProfileId = profileId;
        FeedId = feedId;
        LikeDate = likeDate;
        CompanyId = companyId;
    }

    [Required]
    public Guid Id { get; init; }

    [Required]
    public Guid ProfileId { get; init; }

    [Required]
    public Guid FeedId { get; init; }

    [Required]
    public DateTime LikeDate { get; init; }

    [Required]
    public Guid CompanyId { get; init; }
}
