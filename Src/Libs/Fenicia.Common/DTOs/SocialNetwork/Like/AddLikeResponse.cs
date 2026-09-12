using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Like;

public record AddLikeResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid FeedId,
    [Required] DateTime LikeDate,
    [Required] Guid CompanyId);
