using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Company;

namespace Fenicia.Common.DTOs.Auth.Register;

public class RegisterRequest()
{
    public RegisterRequest(string email, string password, string name, CompanyRequest company)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Company = company;
    }

    [Required]
    [EmailAddress]
    [StringLength(48)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [StringLength(48)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CompanyRequest Company { get; set; } = new();
}
