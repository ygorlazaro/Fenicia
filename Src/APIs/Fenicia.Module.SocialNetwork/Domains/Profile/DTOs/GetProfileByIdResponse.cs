using System.ComponentModel.DataAnnotations;

namespace Fenicia.Module.SocialNetwork.Domains.Profile.DTOs;

public record GetProfileByIdResponse(
    [Required] Guid Id,
    [Required] Guid UserId,
    [MaxLength(200)] string? Bio,
    [MaxLength(200)] string? ImageUrl,
    [MaxLength(200)] string? Website,
    [MaxLength(200)] string? Location,
    [MaxLength(200)] string? Phone,
    DateTime? BirthDate);