using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public class ResetPasswordCommand()
{
    public ResetPasswordCommand(string email, string password, string code)
        : this()
    {
        Email = email;
        Password = password;
        Code = code;
    }

    [Required]
    [EmailAddress]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Code { get; set; } = string.Empty;
}
