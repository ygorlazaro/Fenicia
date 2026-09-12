using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record UpdatePasswordCommand(
    Guid UserId,
    [Required] [StringLength(200)] string Password);