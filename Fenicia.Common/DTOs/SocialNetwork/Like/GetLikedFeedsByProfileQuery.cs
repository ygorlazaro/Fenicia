using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public record GetLikedFeedsByProfileQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid ProfileId = default);
