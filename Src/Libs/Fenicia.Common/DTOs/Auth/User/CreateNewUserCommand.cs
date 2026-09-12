using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record CreateNewUserCommand(
    [Required] [MaxLength(200)] string Email,
    [Required] [MaxLength(200)] string Password,
    [Required] [MaxLength(200)] string Name,
    CreateNewUserCompanyCommand Company);