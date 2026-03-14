namespace Fenicia.Auth.Domains.ForgotPassword.Commands;

/// <summary>
/// Command to initiate the forgot password process.
/// </summary>
public record AddForgotPasswordCommand(string Email);
