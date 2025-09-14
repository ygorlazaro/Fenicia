namespace Fenicia.Common.Database.Requests;

using System.ComponentModel.DataAnnotations;

public record TokenRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [StringLength(maximumLength: 256, ErrorMessage = "Email must not exceed 256 characters")]
    [Display(Name = "Email Address")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [StringLength(maximumLength: 100, MinimumLength = 6, ErrorMessage = "Password must be between 6 and 100 characters")]
    [Display(Name = "Password")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "CNPJ is required")]
    [StringLength(maximumLength: 14, MinimumLength = 14, ErrorMessage = "CNPJ must be exactly 14 digits")]
    [Display(Name = "CNPJ")]
    public string Cnpj { get; set; } = null!;
}
