namespace Fenicia.Auth.Domains.ForgotPassword.DTOs.Commands;

public record AddForgotPasswordCommand(string Email, string? IpAddress = null, string? UserAgent = null);
