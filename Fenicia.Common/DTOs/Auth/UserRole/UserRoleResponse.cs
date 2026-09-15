using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

public class UserRoleResponse()
{
    public UserRoleResponse(Guid id, string role, CompanyResponse company)
        : this()
    {
        Id = id;
        Role = role;
        Company = company;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public CompanyResponse Company { get; set; } = new();
}
