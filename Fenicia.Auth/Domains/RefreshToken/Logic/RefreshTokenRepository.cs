using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.RefreshToken.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.RefreshToken.Logic;

public class RefreshTokenRepository(AuthContext authContext) : IRefreshTokenRepository
{
    public void Add(RefreshTokenModel refreshToken)
    {
        authContext.RefreshTokens.Add(refreshToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await authContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken,
        CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;

        var query =
            from token in authContext.RefreshTokens
            where
                token.UserId == userId
                && now < token.ExpirationDate
                && token.Token == refreshToken
                && token.IsActive
            select token.Id;

        return await query.AnyAsync(cancellationToken);
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken)
    {
        var refreshTokenModel = await authContext.RefreshTokens.FirstOrDefaultAsync(token =>
            token.Token == refreshToken,
            cancellationToken
        );

        if (refreshTokenModel == null)
        {
            return;
        }

        refreshTokenModel.IsActive = false;

        await SaveChangesAsync(cancellationToken);
    }
}
