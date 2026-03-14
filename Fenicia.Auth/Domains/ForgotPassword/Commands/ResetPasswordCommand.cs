namespace Fenicia.Auth.Domains.ForgotPassword.Commands;

/// <summary>
/// Command to reset user password using a verification code.
/// </summary>
public sealed record ResetPasswordCommand(
    /// <summary>
    /// The user's email address.
    /// </summary>
    string Email,
    /// <summary>
    /// The new password to set.
    /// </summary>
    string Password,
    /// <summary>
    /// The verification code sent to the user's email.
    /// </summary>
    string Code);