using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public record GetSharesResponse(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    [MaxLength(200)] string? Text,
    [Required] Guid CompanyId,
    [Required] Guid ProfileId,
    [Required] DateTime ShareDate);
