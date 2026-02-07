namespace Fenicia.Common.Data.Requests.Auth;

public record TokenRequest
{
    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;
}