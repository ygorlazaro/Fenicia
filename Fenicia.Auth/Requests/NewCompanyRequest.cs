using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

public class NewCompanyRequest
{
    [MaxLength(50)]
    [Required]
    public string Name { get; set; } = null!;
    
    [Required]
    [MaxLength(14)]
    public string CNPJ { get; set; } = null!;
}