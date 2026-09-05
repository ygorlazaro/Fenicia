using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikedFeedsByProfileQuery(
    int Page = 1,
    int PerPage = 10,
    [Required] Guid ProfileId = default);
