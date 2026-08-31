using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record AddLikeResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [Required] Guid FeedId,
    [Required] DateTime LikeDate,
    [Required] Guid CompanyId);
