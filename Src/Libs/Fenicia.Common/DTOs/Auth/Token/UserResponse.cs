using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public record UserResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Email);