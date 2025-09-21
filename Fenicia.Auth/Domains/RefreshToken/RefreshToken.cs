namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshToken
{
    public string Token { get; set; } = null!;

    public DateTime ExpirationDate { get; set; } = DateTime.UtcNow.AddDays(7);

    public Guid UserId { get; set; }

    public bool IsActive { get; set; } = true;
}
