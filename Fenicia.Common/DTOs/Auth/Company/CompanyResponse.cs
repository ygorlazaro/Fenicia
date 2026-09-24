using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

public class CompanyResponse()
{
    public CompanyResponse(Guid id, string name, string cnpj, string? role = null)
        : this()
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
        Role = role;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [Length(14, 14)]
    public string Cnpj { get; set; } = string.Empty;

    [MaxLength(10)]
    public string? Role { get; set; } = string.Empty;
}
