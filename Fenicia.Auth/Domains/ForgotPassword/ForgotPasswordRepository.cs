using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

public class ForgotPasswordRepository(AuthContext context, ILogger<ForgotPasswordRepository> logger) : IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var query = context.ForgottenPasswords
                .Where(fp => fp.UserId == userId && fp.Code == code && fp.IsActive && fp.ExpirationDate >= now);

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                logger.LogWarning("No active forgot password record found for user {UserID} with code {Code}", userId, code);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving forgot password record for user {UserID}", userId);
            throw;
        }
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var forgotPassword = await context.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

            if (forgotPassword is null)
            {
                logger.LogWarning("Forgot password record {ID} not found for invalidation", id);

                return;
            }

            forgotPassword.IsActive = false;
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully invalidated forgot password record {ID}", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error invalidating forgot password record {ID}", id);
            throw;
        }
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            context.ForgottenPasswords.Add(forgotPassword);
            await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully saved forgot password record for user {UserID}", forgotPassword.UserId);

            return forgotPassword;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving forgot password record for user {UserID}", forgotPassword.UserId);
            throw;
        }
    }
}
