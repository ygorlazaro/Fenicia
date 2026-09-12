using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record UpdateUserCommand(
    Guid UserId,
    [StringLength(48)] string? Name = null,
    [EmailAddress] [StringLength(48)] string? Email = null,
    List<UpdateUserRoleCommand>? CompaniesRoles = null);