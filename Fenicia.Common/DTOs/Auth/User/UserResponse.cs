using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.UserRole;

namespace Fenicia.Common.DTOs.Auth.User;

public class UserResponse()
{
    public UserResponse(Guid id, string name, string email, CompanyResponse companyResponse)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Company = companyResponse;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public CompanyResponse Company { get; set; } = null!;
}
