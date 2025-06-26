using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.ForgotPassword.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword.Logic;

/// <summary>
/// Repository for managing forgot password functionality and database operations
/// </summary>
/// <param name="authContext">The authentication database context</param>
public class ForgotPasswordRepository(AuthContext authContext, ILogger<ForgotPasswordRepository> logger) : IForgotPasswordRepository
{

    /// <summary>
    /// Retrieves a forgot password model based on user ID and verification code
    /// </summary>
    /// <param name="userId">The ID of the user requesting password reset</param>
    /// <param name="code">The verification code sent to the user</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The forgot password model if found and valid; otherwise null</returns>
    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code,
        CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var query = from forgotPassword in authContext.ForgottenPasswords
                        where forgotPassword.UserId == userId && forgotPassword.Code == code
                        && forgotPassword.IsActive
                        && forgotPassword.ExpirationDate >= now
                        select forgotPassword;

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                logger.LogWarning("No active forgot password record found for user {UserId} with code {Code}", userId, code);
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving forgot password record for user {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    /// Invalidates a forgot password code
    /// </summary>
    /// <param name="id">The ID of the forgot password record to invalidate</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>A task representing the asynchronous operation</returns>
    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var forgotPassword =
                await authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

            if (forgotPassword is null)
            {
                logger.LogWarning("Forgot password record {Id} not found for invalidation", id);
                return;
            }

            forgotPassword.IsActive = false;
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully invalidated forgot password record {Id}", id);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error invalidating forgot password record {Id}", id);
            throw;
        }
    }

    /// <summary>
    /// Saves a new forgot password request
    /// </summary>
    /// <param name="forgotPassword">The forgot password model to save</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The saved forgot password model</returns>
    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(
        ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            authContext.ForgottenPasswords.Add(forgotPassword);
            await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Successfully saved forgot password record for user {UserId}", forgotPassword.UserId);

            return forgotPassword;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving forgot password record for user {UserId}", forgotPassword.UserId);
            throw;
        }
    }
}
