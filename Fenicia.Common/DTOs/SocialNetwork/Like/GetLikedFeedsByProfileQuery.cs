using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class GetLikedFeedsByProfileQuery()
{
    public GetLikedFeedsByProfileQuery(int page = 1, int perPage = 10, Guid profileId = default)
        : this()
    {
        Page = page;
        PerPage = perPage;
        ProfileId = profileId;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    [Required]
    public Guid ProfileId { get; set; }
}
