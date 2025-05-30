using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

public class NewUserRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    [MaxLength(48)]
    public string Password { get; set; }= null!;
    
    [Required]
    [MaxLength(32)]
    public string Name { get; set; }= null!;
    
    [Required]
    public NewCompanyRequest Company { get; set; } = null!;
}
    
