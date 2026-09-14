using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public record GetBlockedResponse(
    [Required] Guid Id,
    [Required] Guid BlockedProfileId,
    [Required] DateTime BlockDate,
    [MaxLength(200)] string? Reason);
