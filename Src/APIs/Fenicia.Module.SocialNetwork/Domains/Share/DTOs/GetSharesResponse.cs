using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record GetSharesResponse(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    [MaxLength(200)] string? Text,
    [Required] Guid CompanyId,
    [Required] Guid ProfileId,
    [Required] DateTime ShareDate);
