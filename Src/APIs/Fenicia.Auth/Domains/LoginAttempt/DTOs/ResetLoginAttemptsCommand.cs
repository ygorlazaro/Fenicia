using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.LoginAttempt.DTOs;

public sealed record ResetLoginAttemptsCommand([Required] [MaxLength(200)] string Email);