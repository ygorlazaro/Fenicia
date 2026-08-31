using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.User.DTOs;

public record DeleteUserCommand([Required] Guid UserId);
