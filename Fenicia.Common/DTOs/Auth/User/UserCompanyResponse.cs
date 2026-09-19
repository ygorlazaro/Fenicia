using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public class UserCompanyResponse()
{
    public UserCompanyResponse(Guid id, string name, string cnpj)
        : this()
    {
        Id = id;
        Name = name;
        Cnpj = cnpj;
    }

    [Required]
    public Guid Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Required]
    [MaxLength(200)]
    public string Cnpj { get; set; } = string.Empty;
}
