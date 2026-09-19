using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.ForgotPassword;

public class ResetPasswordRequest()
{
    public ResetPasswordRequest(string email, string password, string code)
        : this()
    {
        Email = email;
        Password = password;
        Code = code;
    }

    [Required]
    [EmailAddress]
    [MaxLength(48)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(6)]
    public string Code { get; set; } = string.Empty;
}
