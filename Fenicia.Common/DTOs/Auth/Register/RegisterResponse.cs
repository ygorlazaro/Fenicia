using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Common.DTOs.Auth.Register;

public class RegisterResponse()
{
    public RegisterResponse(Guid id, string name, string email, CreateNewUserCompanyResponse company)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        Company = company;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    [Required]
    public CreateNewUserCompanyResponse Company { get; set; } = default!;
}
