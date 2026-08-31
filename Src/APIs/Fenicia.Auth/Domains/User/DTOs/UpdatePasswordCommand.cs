using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdatePasswordCommand(
    Guid UserId,
    [Required][StringLength(200)] string Password);
