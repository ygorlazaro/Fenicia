using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record UpdateUserRoleCommand([Required] Guid CompanyId, [Required] Guid RoleId);