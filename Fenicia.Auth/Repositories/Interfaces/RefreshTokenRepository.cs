using Fenicia.Auth.Contexts;
using Fenicia.Auth.Contexts.Models;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Repositories.Interfaces;

public class RefreshTokenRepository(AuthContext authContext): IRefreshTokenRepository
{
    public void Add(RefreshTokenModel refreshToken)
    {
        refreshToken.Created = DateTime.Now;
        
        authContext.RefreshTokens.Add(refreshToken);
    }

    public async Task SaveChangesAsync()
    {
        await authContext.SaveChangesAsync();
    }

    public async Task<bool> ValidateTokenAsync(Guid userId, string refreshToken)
    {
        var now = DateTime.Now;
        
        var query = from token in authContext.RefreshTokens
            where token.UserId == userId
                  && now < token.ExpirationDate 
                  && token.Token == refreshToken
                  && token.IsActive
            select token.Id;
        
        return await query.AnyAsync();
    }

    public async Task InvalidateRefreshTokenAsync(string refreshToken)
    {
        var refreshTokenModel = await authContext.RefreshTokens.FirstOrDefaultAsync(token => token.Token == refreshToken);

        if (refreshTokenModel == null)
        {
            return;
        }
        
        refreshTokenModel.IsActive = false;
        
        await SaveChangesAsync();
    }
}