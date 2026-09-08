using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserPasswordCommand(
    [Required] Guid UserId,
    string? CurrentPassword,
    [Required] [MaxLength(200)] string NewPassword,
    [Required] [MaxLength(200)] string ConfirmPassword);