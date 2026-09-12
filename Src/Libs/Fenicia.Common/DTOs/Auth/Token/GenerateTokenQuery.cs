using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Token;

public record GenerateTokenQuery([Required] [MaxLength(200)] string Email, [Required] [MaxLength(200)] string Password);