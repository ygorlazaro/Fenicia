using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record UpdateProfileCommand(
    [Required] Guid Id,
    [MaxLength(200)] string? Bio,
    [MaxLength(200)] string? ImageUrl,
    [MaxLength(200)] string? Website,
    [MaxLength(200)] string? Location,
    [MaxLength(200)] string? Phone,
    DateTime? BirthDate);
