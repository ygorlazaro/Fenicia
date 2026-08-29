namespace Fenicia.Auth.Domains.ForgotPassword.DTOs;

public record AddForgotPasswordCommand(string Email, string? IpAddress = null, string? UserAgent = null);
