using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

public class GetCompaniesByUserResponse()
{
    public GetCompaniesByUserResponse(Guid id, string name, string cnpj, string role)
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
    [MaxLength(14)]
    public string Cnpj { get; set; } = string.Empty;

    [Required]
    [MaxLength(10)]
    public string Role { get; set; } = string.Empty;
}