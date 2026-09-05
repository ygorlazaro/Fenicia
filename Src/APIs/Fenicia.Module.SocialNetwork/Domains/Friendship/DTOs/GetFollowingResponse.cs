using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowingResponse(
    [Required] Guid Id,
    [Required] Guid TargetProfileId,
    [Required] DateTime FollowDate);
