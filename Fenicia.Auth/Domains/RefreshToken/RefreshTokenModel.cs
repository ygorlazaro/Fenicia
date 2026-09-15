namespace Fenicia.Auth.Domains.RefreshToken;

public class RefreshTokenModel()
{
    public RefreshTokenModel(string token, DateTime expirationDate, Guid userId, bool isActive = true)
        : this()
    {
        Token = token;
        ExpirationDate = expirationDate;
        UserId = userId;
        IsActive = isActive;
    }

    public string Token { get; set; } = string.Empty;
    public DateTime ExpirationDate { get; set; }
    public Guid UserId { get; set; }
    public bool IsActive { get; set; } = true;
}
