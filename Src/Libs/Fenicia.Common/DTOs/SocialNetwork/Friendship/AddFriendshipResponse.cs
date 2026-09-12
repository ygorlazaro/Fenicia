using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Friendship;

public record AddFriendshipResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid TargetProfileId,
    [Required] DateTime FollowDate,
    bool IsActive);
