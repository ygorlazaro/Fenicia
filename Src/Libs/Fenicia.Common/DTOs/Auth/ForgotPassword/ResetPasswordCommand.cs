using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public sealed record ResetPasswordCommand(
    [Required] [MaxLength(200)] string Email,
    [Required] [MaxLength(200)] string Password,
    [Required] [MaxLength(200)] string Code);