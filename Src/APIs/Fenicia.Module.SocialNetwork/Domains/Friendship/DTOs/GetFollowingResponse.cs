using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowingResponse(
    [Required] Guid Id,
    [Required] Guid TargetUserId,
    [Required] DateTime FollowDate);