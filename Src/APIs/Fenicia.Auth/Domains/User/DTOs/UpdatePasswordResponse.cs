using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdatePasswordResponse([Required] Guid Id, [Required][MaxLength(200)] string Name, [Required][MaxLength(200)] string Email);
