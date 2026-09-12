using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record GetFollowingResponse(
    [Required] Guid Id,
    [Required] Guid TargetProfileId,
    [Required] DateTime FollowDate);
