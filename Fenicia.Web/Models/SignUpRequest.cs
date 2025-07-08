using System.ComponentModel.DataAnnotations;

namespace Fenicia.Web.Models;

public class SignUpRequest
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public CompanyRequest Company { get; set; } = new();
}

public class CompanyRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    [Required]
    public string Cnpj { get; set; } = string.Empty;
}
