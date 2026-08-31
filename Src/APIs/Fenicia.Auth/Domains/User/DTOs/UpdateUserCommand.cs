using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserCommand(
    Guid UserId,
    [StringLength(48)] string? Name = null,
    [EmailAddress][StringLength(48)] string? Email = null,
    List<UpdateUserRoleCommand>? CompaniesRoles = null);
