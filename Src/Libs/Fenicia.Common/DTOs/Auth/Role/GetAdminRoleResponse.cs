using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.Role;

public record GetAdminRoleResponse([Required] Guid Id, [Required] [MaxLength(200)] string Name);