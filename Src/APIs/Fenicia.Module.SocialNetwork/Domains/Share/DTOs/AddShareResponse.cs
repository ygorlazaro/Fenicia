using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record AddShareResponse(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    [MaxLength(200)] string? Text,
    [Required] Guid CompanyId,
    [Required] Guid UserId,
    [Required] DateTime ShareDate);
