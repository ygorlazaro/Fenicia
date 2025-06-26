using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.RefreshToken.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.RefreshToken.Logic;

/// <summary>
/// Repository for managing refresh tokens in the database.
/// </summary>
public sealed class RefreshTokenRepository(AuthContext authContext) : IRefreshTokenRepository
{
    /// <summary>
    /// Adds a new refresh token to the database context.
    /// </summary>
    /// <param name="refreshToken">The refresh token model to add.</param>
    public void Add(RefreshTokenModel refreshToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);
        authContext.RefreshTokens.Add(refreshToken);
    }

    /// <summary>
    /// Saves all pending changes in the database context.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        try
        {
            await authContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateException ex)
        {
            throw new InvalidOperationException("Failed to save changes to the database.", ex);
        }
    }

    /// <summary>
    /// Validates if a refresh token is valid for a specific user.
    /// </summary>
    /// <param name="userId">The user identifier.</param>
    /// <param name="refreshToken">The refresh token to validate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the token is valid, false otherwise.</returns>
    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to validate refresh token.", ex);
        }
    }

    /// <summary>
    /// Invalidates a refresh token.
    /// </summary>
    /// <param name="refreshToken">The refresh token to invalidate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvalidateRefreshTokenAsync(string refreshToken,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(refreshToken);

        try
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
        catch (Exception ex)
        {
            throw new InvalidOperationException("Failed to invalidate refresh token.", ex);
        }
    }
}
