using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserRoleCommand([Required] Guid CompanyId, [Required] Guid RoleId);