using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Block.DTOs;

public record GetBlockedResponse(
    [Required] Guid Id,
    [Required] Guid BlockedUserId,
    [Required] DateTime BlockDate,
    [MaxLength(200)] string? Reason);