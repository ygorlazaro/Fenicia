using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record GetFollowersResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] DateTime FollowDate);
