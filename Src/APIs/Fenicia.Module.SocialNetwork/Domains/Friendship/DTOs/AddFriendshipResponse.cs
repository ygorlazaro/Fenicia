using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record AddFriendshipResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] Guid TargetUserId,
    [Required] DateTime FollowDate,
    bool IsActive);