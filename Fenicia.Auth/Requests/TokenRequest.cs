using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

public record TokenRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(48)]
    public string Password { get; set; }= null!;
    
    [Required]
    [MaxLength(14)]
    public string CNPJ { get; set; } = null!;
}
    
