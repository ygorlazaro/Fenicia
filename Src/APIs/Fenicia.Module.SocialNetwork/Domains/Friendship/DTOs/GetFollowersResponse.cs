using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Friendship.DTOs;

public record GetFollowersResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] DateTime FollowDate);