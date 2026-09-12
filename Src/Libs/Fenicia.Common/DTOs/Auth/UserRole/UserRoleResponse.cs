using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.UserRole;

public record UserRoleResponse([Required] Guid Id, [Required] [MaxLength(200)] string Role, CompanyResponse Company);