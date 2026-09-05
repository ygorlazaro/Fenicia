using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record AddBlockResponse(
    [Required] Guid Id,
    [Required] Guid ProfileId,
    [Required] Guid BlockedProfileId,
    [Required] DateTime BlockDate,
    [MaxLength(200)] string? Reason,
    bool IsActive);
