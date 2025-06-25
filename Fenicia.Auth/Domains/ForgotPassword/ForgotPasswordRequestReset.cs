using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRequestReset
{
    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string Code { get; set; } = null!;

    [Required]
    [MaxLength(48)]
    public string Password { get; set; } = null!;
}
