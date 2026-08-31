using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Feed.DTOs;

public record GetFeedByIdResponse(
    [Required] Guid Id,
    [Required] DateTime Date,
    [Required][MaxLength(200)] string Text,
    [Required] Guid UserId,
    [Required] Guid CompanyId,
    int CommentsCount,
    int LikesCount,
    int SharesCount);
