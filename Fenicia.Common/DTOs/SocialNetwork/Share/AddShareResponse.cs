using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public record AddShareResponse(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    string? Text,
    [Required] Guid CompanyId,
    [Required] Guid ProfileId,
    [Required] DateTime ShareDate,
    [Required] Guid ShareFeedId);
