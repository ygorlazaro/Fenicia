using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Feed;

public record GetFeedsByProfileQuery(
    int Page = 1,
    int PerPage = 20,
    [Required] Guid ProfileId = default,
    string? Query = null,
    string? Sort = null);
