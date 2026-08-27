namespace Fenicia.Auth.Domains.ForgotPassword.DTOs.Commands;

public sealed record ResetPasswordCommand(
    string Email,
    string Password,
    string Code);
