namespace Fenicia.Common.DTOs.Auth.RefreshToken;

public class UpdateRefreshTokenCommand()
{
    public UpdateRefreshTokenCommand(bool isActive)
        : this()
    {
        IsActive = isActive;
    }

    public bool IsActive { get; set; }
}
