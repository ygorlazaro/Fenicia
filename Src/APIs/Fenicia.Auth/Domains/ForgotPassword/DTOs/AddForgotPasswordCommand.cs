using System.ComponentModel.DataAnnotations;

namespace Fenicia.Auth.Domains.ForgotPassword.DTOs;

public record AddForgotPasswordCommand(
    [Required] [MaxLength(200)] string Email,
    [MaxLength(200)] string? IpAddress = null,
    [MaxLength(200)] string? UserAgent = null);