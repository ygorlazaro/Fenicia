using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record UpdateUserPasswordCommand([Required] Guid UserId, [Required] [MaxLength(200)] string Password);