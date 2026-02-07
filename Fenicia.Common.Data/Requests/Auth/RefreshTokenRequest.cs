namespace Fenicia.Common.Data.Requests.Auth;

public class RefreshTokenRequest
{
    public string RefreshToken { get; set; } = string.Empty;

    public Guid UserId
    {
        get; set;
    }
}