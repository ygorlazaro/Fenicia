namespace Fenicia.Auth.Domains.User.Data;

using System.ComponentModel.DataAnnotations;

using Company.Data;

public class UserRequest
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address format")]
    [MaxLength(length: 48, ErrorMessage = "Email cannot exceed 48 characters")]
    public string Email { get; set; } = null!;

    [Required(ErrorMessage = "Password is required")]
    [MinLength(length: 8, ErrorMessage = "Password must be at least 8 characters")]
    [MaxLength(length: 200, ErrorMessage = "Password cannot exceed 200 characters")]
    public string Password { get; set; } = null!;

    [Required(ErrorMessage = "Name is required")]
    [MaxLength(length: 48, ErrorMessage = "Name cannot exceed 48 characters")]
    public string Name { get; set; } = null!;

    [Required(ErrorMessage = "Company information is required")]
    public CompanyRequest Company { get; set; } = null!;
}
