using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikesResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] Guid FeedId,
    [Required] DateTime LikeDate);