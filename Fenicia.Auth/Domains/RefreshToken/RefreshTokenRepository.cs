namespace Fenicia.Auth.Domains.RefreshToken;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public sealed class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly AuthContext authContext;

    public RefreshTokenRepository(AuthContext authContext)
    {
        this.authContext = authContext;
    }

    public void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        this.authContext.RefreshTokens.Add(refreshToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await this.authContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Failed to save changes to the database.", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var now = DateTime.UtcNow;

            var query = from token in this.authContext.RefreshTokens where token.UserId == userId && now < token.ExpirationDate && token.Token == refreshToken && token.IsActive select token.Id;

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate refresh token.", ex);
        }
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var refreshTokenModel = await this.authContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);

            if (refreshTokenModel == null)
            {
                return;
            }

            refreshTokenModel.IsActive = false;

            await this.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to invalidate refresh token.", ex);
        }
    }
}
