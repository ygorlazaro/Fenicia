namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

using Contexts;

using Data;

using Microsoft.EntityFrameworkCore;

public class ForgotPasswordRepository(AuthContext authContext, ILogger<ForgotPasswordRepository> logger) : IForgotPasswordRepository
{
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var query = from forgotPassword in authContext.ForgottenPasswords where forgotPassword.UserId == userId && forgotPassword.Code == code && forgotPassword.IsActive && forgotPassword.ExpirationDate >= now select forgotPassword;

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                logger.LogWarning(message: "No active forgot password record found for user {UserId} with code {Code}", userId, code);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving forgot password record for user {UserId}", userId);
            throw;
        }
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var forgotPassword = await authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

            if (forgotPassword is null)
            {
                logger.LogWarning(message: "Forgot password record {Id} not found for invalidation", id);
                return;
            }

            forgotPassword.IsActive = false;
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(message: "Successfully invalidated forgot password record {Id}", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error invalidating forgot password record {Id}", id);
            throw;
        }
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            authContext.ForgottenPasswords.Add(forgotPassword);
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(message: "Successfully saved forgot password record for user {UserId}", forgotPassword.UserId);

            return forgotPassword;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error saving forgot password record for user {UserId}", forgotPassword.UserId);
            throw;
        }
    }
}
