using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetAllFeedResponse(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required] [MaxLength(200)] string Text,
    [Required] Guid ProfileId,
    [Required] Guid CompanyId,
    int TotalLikes,
    int TotalComments,
    int TotalShares,
    Guid? OriginalFeedId,
    [MaxLength(64)] string? AuthorUserName);
