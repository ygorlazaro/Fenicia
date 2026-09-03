using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateUserRoleCommand([Required] Guid CompanyId, [Required] Guid RoleId);