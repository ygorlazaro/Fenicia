using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.UserRole.DTOs;

public record CompanyResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Cnpj);