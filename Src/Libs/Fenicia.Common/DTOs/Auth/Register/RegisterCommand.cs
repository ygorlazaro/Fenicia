using System.ComponentModel.DataAnnotations;
using Fenicia.Common.DTOs.Auth.User;

namespace Fenicia.Common.DTOs.Auth.Register;

public record RegisterCommand(
    [Required]
    [EmailAddress]
    [StringLength(48)]
    string Email,
    [Required] [StringLength(200)] string Password,
    [Required] [StringLength(48)] string Name,
    CreateNewUserCompanyCommand Company);