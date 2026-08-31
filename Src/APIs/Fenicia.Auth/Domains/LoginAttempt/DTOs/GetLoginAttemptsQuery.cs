using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.LoginAttempt.DTOs;

public sealed record GetLoginAttemptsQuery([Required][MaxLength(200)] string Email);
