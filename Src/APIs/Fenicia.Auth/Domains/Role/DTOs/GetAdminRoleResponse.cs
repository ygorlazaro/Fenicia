using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.Role.DTOs;

public record GetAdminRoleResponse([Required] Guid Id, [Required][MaxLength(200)] string Name);
