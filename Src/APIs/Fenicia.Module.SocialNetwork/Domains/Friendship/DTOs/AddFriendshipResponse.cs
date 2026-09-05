using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record AddFriendshipResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid TargetProfileId,
    [Required] DateTime FollowDate,
    bool IsActive);
