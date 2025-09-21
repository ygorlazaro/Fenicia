namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public record TokenRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    [StringLength(256, ErrorMessage = "Email must be at most 256 characters long")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters long")]
    public string Password { get; set; } = null!;
}
