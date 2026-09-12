using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record UpdateUserPasswordCommand(
    [Required] Guid UserId,
    string? CurrentPassword,
    [Required] [MaxLength(200)] string NewPassword,
    [Required] [MaxLength(200)] string ConfirmPassword);