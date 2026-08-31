using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Share.DTOs;

public record ShareCommand(
    [Required] Guid Id,
    [Required] Guid OriginalFeedId,
    [MaxLength(200)] string? Text);
