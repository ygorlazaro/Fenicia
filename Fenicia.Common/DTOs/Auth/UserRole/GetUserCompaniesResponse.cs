using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

public class GetUserCompaniesResponse()
{
    public GetUserCompaniesResponse(Guid id, string role, Guid companyId, string companyName, string cnpj)
        : this()
    {
        Id = id;
        Role = role;
        CompanyId = companyId;
        CompanyName = companyName;
        Cnpj = cnpj;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Role { get; set; } = string.Empty;

    [Required]
    public Guid CompanyId { get; set; }

    [Required]
    [MaxLength(200)]
    public string CompanyName { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;
}
