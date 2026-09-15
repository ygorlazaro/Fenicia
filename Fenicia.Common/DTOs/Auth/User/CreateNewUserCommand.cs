using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class CreateNewUserCommand()
{
    public CreateNewUserCommand(string email, string password, string name, CreateNewUserCompanyCommand company)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Company = company;
    }

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Password { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CreateNewUserCompanyCommand Company { get; set; } = new();
}
