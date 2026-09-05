using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record AddShareResponse(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    string? Text,
    [Required] Guid CompanyId,
    [Required] Guid ProfileId,
    [Required] DateTime ShareDate,
    [Required] Guid ShareFeedId);
