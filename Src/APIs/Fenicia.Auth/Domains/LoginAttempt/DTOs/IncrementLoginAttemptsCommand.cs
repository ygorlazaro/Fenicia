using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.LoginAttempt.DTOs;

public sealed record IncrementLoginAttemptsCommand([Required] [MaxLength(200)] string Email);