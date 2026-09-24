using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

public class CompanyRequest()
{
    public CompanyRequest(string name, string cnpj)
        : this()
    {
        Name = name;
        Cnpj = cnpj;
    }

    public Guid Id { get; set; }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(14)]
    public string Cnpj { get; set; } = string.Empty;

}
