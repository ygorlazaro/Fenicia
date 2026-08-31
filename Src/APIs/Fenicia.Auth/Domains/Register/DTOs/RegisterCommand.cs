using System.ComponentModel.DataAnnotations;
using Fenicia.Auth.Domains.User.DTOs;

namespace Fenicia.Auth.Domains.Register.DTOs;

public record RegisterCommand(
    [Required][EmailAddress][StringLength(48)] string Email,
    [Required][StringLength(200)] string Password,
    [Required][StringLength(48)] string Name,
    CreateNewUserCompanyCommand Company);
