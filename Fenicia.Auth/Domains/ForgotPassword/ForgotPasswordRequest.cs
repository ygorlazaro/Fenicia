using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRequest
{
    [Required]
    public string Email { get; set; } = null!;
}
