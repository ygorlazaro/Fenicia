using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Like.DTOs;

public record GetLikedFeedsResponse(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(512)] string Text,
    [Required] Guid ProfileId,
    [Required] Guid CompanyId,
    int TotalLikes,
    int TotalComments,
    int TotalShares);
