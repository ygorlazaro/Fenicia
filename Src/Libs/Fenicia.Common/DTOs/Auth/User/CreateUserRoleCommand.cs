using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record CreateUserRoleCommand([Required] Guid CompanyId, [Required] Guid RoleId);