namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRequestReset
{
    public string Email { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string Password { get; set; } = null!;
}
