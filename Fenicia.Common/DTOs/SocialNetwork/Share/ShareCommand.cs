using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Share;

public record ShareCommand(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    [MaxLength(200)] string? Text);