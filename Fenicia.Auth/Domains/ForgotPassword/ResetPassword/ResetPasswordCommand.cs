namespace Fenicia.Auth.Domains.ForgotPassword.ResetPassword;

public sealed record ResetPasswordCommand(string Email, string Password, string Code);