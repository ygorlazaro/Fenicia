using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record GetByEmailResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Email,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Password);