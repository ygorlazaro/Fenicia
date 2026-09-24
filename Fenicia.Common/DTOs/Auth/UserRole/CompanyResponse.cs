using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

public class CompanyResponse
{
    public CompanyResponse()
    {
    }

    public CompanyResponse(Guid id, string name, string cnpj, bool isActive = true)
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
        IsActive = isActive;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;
}
