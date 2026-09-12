using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public record UpdateProfileCommand(
    [Required] Guid Id,
    [MaxLength(64)] string? UserName,
    [MaxLength(200)] string? Bio,
    Guid? UploadId,
    [MaxLength(200)] string? Website,
    [MaxLength(200)] string? Location,
    [MaxLength(200)] string? Phone,
    DateTime? BirthDate);