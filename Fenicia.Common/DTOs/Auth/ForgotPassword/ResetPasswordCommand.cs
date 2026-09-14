using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public class ResetPasswordCommand(string email, string password, string code)
{
    public ResetPasswordCommand()
        : this(string.Empty, string.Empty, string.Empty)
    {
    }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = email;

    [Required]
    [MaxLength(200)]
    public string Code { get; set; } = code;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = password;
}
