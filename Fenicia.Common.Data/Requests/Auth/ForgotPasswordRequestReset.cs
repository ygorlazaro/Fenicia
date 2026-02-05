using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Data.Requests.Auth;

public class ForgotPasswordRequestReset
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Verification code is required")]
    [MinLength(6, ErrorMessage = "Verification code must be at least 6 characters")]
    public string Code { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(8, ErrorMessage = "Password must be at least 8 characters")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$", ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number and one special character")]
    public string Password { get; set; } = null!;
}
