using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetFeedByIdResponse(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(200)] string Text,
    [Required] Guid ProfileId,
    [Required] Guid CompanyId,
    int TotalLikes,
    int TotalComments,
    int TotalShares);
