using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public class GetFeedsByProfileQuery()
{
    public GetFeedsByProfileQuery(
        int page = 1,
        int perPage = 20,
        Guid profileId = default,
        string? query = null,
        string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        ProfileId = profileId;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; }
    public int PerPage { get; set; }

    [Required]
    public Guid ProfileId { get; set; }

    public string? Query { get; set; }
    public string? Sort { get; set; }
}
