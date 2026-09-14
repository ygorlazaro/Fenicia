using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Common.DTOs.Auth.Register;

public record RegisterCommand
{
    public RegisterCommand(string email, string password, string name, CreateNewUserCompanyCommand company)
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
    public CreateNewUserCompanyCommand Company { get; set; } = new();
}
