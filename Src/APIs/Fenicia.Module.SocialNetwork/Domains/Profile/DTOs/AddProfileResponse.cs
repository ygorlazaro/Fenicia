using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record AddProfileResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [MaxLength(160)] string? Bio,
    [MaxLength(48)] string? ImageUrl,
    [MaxLength(120)] string? Website,
    [MaxLength(64)] string? Location,
    [MaxLength(24)] string? Phone,
    DateTime? BirthDate);
