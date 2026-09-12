using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record GetUserForRefreshResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Email,
    [Required] [MaxLength(200)] string Name);