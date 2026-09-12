using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record CreateUserCommand(
    [Required]
    [EmailAddress]
    [StringLength(48)]
    string Email,
    [Required] [StringLength(200)] string Password,
    [Required] [StringLength(48)] string Name,
    List<CreateUserRoleCommand>? Roles = null);