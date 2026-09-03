using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikesByFeedQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid FeedId = default,
    string? Query = null,
    string? Sort = null);