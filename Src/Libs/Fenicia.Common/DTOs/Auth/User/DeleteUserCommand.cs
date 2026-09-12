using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.User;

public record DeleteUserCommand([Required] Guid UserId);