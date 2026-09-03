using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.UserRole.DTOs;

public record GetUserCompaniesResponse(
    [Required] Guid Id,
    [Required] [MaxLength(200)] string Role,
    [Required] Guid CompanyId,
    [Required] [MaxLength(200)] string CompanyName,
    [Required] [MaxLength(200)] string Cnpj);