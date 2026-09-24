using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.Company;
using Fenicia.Common.DTOs.Auth.Role;

namespace Fenicia.Common.DTOs.Auth.User;

public class UserRequest()
{
    public UserRequest(string email, string password, string name, CompanyRequest company, List<RoleRequest>? roles = null)
        : this()
    {
        Email = email;
        Password = password;
        Name = name;
        Roles = roles;
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

    public List<RoleRequest>? Roles { get; set; }

    public Guid Id { get; set; }

    public CompanyRequest Company { get; set; } = null!;
}
