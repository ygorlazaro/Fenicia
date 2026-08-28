namespace Fenicia.Auth.Domains.ForgotPassword.DTOs;

public sealed record ResetPasswordCommand(
    string Email,
    string Password,
    string Code);
