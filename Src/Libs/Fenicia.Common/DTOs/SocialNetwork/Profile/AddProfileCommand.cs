using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.SocialNetwork.Profile;

public record AddProfileCommand(
    [MaxLength(64)] string? UserName,
    [MaxLength(160)] string? Bio,
    Guid? UploadId,
    [MaxLength(120)] string? Website,
    [MaxLength(64)] string? Location,
    [MaxLength(24)] string? Phone,
    DateTime? BirthDate);
