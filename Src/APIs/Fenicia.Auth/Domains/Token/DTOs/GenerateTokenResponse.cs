using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Token.DTOs;

public record GenerateTokenResponse([Required] Guid Id, [Required][MaxLength(200)] string Name, [Required][MaxLength(200)] string Email);
