using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.Database.Requests.Auth;

public class ForgotPasswordRequest
{
    [Required(ErrorMessage = "Email address is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(256, ErrorMessage = "Email address cannot exceed 256 characters")]
    [RegularExpression(@"^[^@\s]+@[^@\s]+\.[^@\s]+$", ErrorMessage = "Email address format is invalid")]
    public string Email { get; set; } = null!;
}
