namespace Fenicia.Auth.Domains.RefreshToken.Logic;

using Contexts;

using Data;

using Microsoft.EntityFrameworkCore;

public sealed class RefreshTokenRepository(AuthContext authContext) : IRefreshTokenRepository
{
    public void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        authContext.RefreshTokens.Add(refreshToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await authContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException(message: "Failed to save changes to the database.", ex);
        }
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var now = DateTime.UtcNow;

            var query = from token in authContext.RefreshTokens where token.UserId == userId && now < token.ExpirationDate && token.Token == refreshToken && token.IsActive select token.Id;

            return await query.AnyAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(message: "Failed to validate refresh token.", ex);
        }
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
        {
            var refreshTokenModel = await authContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshToken, cancellationToken);

            if (refreshTokenModel == null)
            {
                return;
            }

            refreshTokenModel.IsActive = false;

            await SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException(message: "Failed to invalidate refresh token.", ex);
        }
    }
}
