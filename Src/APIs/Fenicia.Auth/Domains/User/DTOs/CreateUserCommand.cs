using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record CreateUserCommand(
    [Required][EmailAddress][StringLength(48)] string Email,
    [Required][StringLength(200)] string Password,
    [Required][StringLength(48)] string Name,
    List<CreateUserRoleCommand>? Roles = null);
