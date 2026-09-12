using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

public record CompanyResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Name,
    [Required] [MaxLength(200)] string Cnpj);