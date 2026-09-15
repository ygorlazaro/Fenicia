using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public class GenerateTokenResponse()
{
    public GenerateTokenResponse(Guid id, string name, string email, Guid companyId = default, List<string>? roles = null)
        : this()
    {
        Id = id;
        Name = name;
        Email = email;
        CompanyId = companyId;
        Roles = roles;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Email { get; set; } = string.Empty;

    public Guid CompanyId { get; set; }

    public List<string>? Roles { get; set; }
}
