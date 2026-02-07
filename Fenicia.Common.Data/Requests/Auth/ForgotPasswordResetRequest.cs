namespace Fenicia.Common.Data.Requests.Auth;

public class ForgotPasswordResetRequest
{
    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Password { get; set; } = null!;
}