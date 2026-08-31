using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.UserRole.DTOs;

public record UserRoleResponse([Required] Guid Id, [Required][MaxLength(200)] string Role, CompanyResponse Company);
