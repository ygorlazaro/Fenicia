using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class CreateUserCommand()
{
    public CreateUserCommand(string email, string password, string name, List<CreateUserRoleCommand>? roles = null)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Roles = roles;
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

    public List<CreateUserRoleCommand>? Roles { get; set; }
}
