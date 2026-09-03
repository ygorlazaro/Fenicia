using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.ForgotPassword.DTOs;

public sealed record ResetPasswordCommand(
    [Required] [MaxLength(200)] string Email,
    [Required] [MaxLength(200)] string Password,
    [Required] [MaxLength(200)] string Code);