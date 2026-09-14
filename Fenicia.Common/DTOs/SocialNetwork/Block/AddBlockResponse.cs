using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Block;

public record AddBlockResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid BlockedProfileId,
    [Required] DateTime BlockDate,
    [MaxLength(200)] string? Reason,
    bool IsActive);
