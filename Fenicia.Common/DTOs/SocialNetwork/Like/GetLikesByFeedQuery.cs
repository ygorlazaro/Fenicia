using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public record GetLikesByFeedQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid FeedId = default,
    string? Query = null,
    string? Sort = null);