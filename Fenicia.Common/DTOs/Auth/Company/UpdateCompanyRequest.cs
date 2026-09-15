using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Company;

public class UpdateCompanyRequest()
{
    public UpdateCompanyRequest(string name)
        : this()
    {
        Name = name;
    }

    [Required]
    [MaxLength(50)]
    public string Name { get; set; } = string.Empty;
}