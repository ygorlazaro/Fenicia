using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Requests;

/// <summary>
/// Request model for authentication token generation
/// </summary>
public record TokenRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    /// <example>user@example.com</example>
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;

    /// <summary>
    /// User's password
    /// </summary>
    [Required]
    [MaxLength(48)]
    public string Password { get; set; } = null!;

    /// <summary>
    /// Company's CNPJ number
    /// </summary>
    /// <example>12345678000199</example>
    [Required]
    [MaxLength(14)]
    public string Cnpj { get; set; } = null!;
}