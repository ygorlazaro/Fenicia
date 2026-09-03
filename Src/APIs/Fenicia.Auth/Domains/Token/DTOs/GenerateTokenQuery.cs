using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Token.DTOs;

public record GenerateTokenQuery([Required] [MaxLength(200)] string Email, [Required] [MaxLength(200)] string Password);