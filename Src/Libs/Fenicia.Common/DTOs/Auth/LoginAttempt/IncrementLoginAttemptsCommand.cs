using System.ComponentModel.DataAnnotations;

namespace Fenicia.Common.DTOs.Auth.LoginAttempt;

public sealed record IncrementLoginAttemptsCommand([Required] [MaxLength(200)] string Email);