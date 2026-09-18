using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public class GetLikesByFeedQuery()
{
    public GetLikesByFeedQuery(int page = 1, int perPage = 10, Guid feedId = default, string? query = null, string? sort = null)
        : this()
    {
        Page = page;
        PerPage = perPage;
        FeedId = feedId;
        Query = query;
        Sort = sort;
    }

    public int Page { get; set; } = 1;

    public int PerPage { get; set; } = 10;

    [Required]
    public Guid FeedId { get; set; }

    public string? Query { get; set; }

    public string? Sort { get; set; }
}
