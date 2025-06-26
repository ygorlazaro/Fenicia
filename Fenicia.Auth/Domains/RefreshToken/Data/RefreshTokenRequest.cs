namespace Fenicia.Auth.Domains.RefreshToken.Data;

public class RefreshTokenRequest
{
    public string AccessToken { get; set; } = null!;

    public string RefreshToken { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid CompanyId { get; set; }
}
