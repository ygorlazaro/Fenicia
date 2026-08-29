namespace Fenicia.Auth.Domains.ForgotPassword.DTOs;

public record ResetForgotPasswordCommand(string Email, string Password, string Code);
