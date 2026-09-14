using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record CreateNewUserCompanyCommand
{
    public CreateNewUserCompanyCommand()
    {
    }

    public CreateNewUserCompanyCommand(string cnpj, string name)
    {
        Cnpj = cnpj;
        Name = name;
    }

    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;
}
