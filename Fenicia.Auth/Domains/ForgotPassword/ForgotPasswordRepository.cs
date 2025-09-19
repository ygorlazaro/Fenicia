namespace Fenicia.Auth.Domains.ForgotPassword;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class ForgotPasswordRepository : IForgotPasswordRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<ForgotPasswordRepository> logger;

    public ForgotPasswordRepository(AuthContext authContext, ILogger<ForgotPasswordRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var query = from forgotPassword in this.authContext.ForgottenPasswords where forgotPassword.UserId == userId && forgotPassword.Code == code && forgotPassword.IsActive && forgotPassword.ExpirationDate >= now select forgotPassword;

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                this.logger.LogWarning("No active forgot password record found for user {UserID} with code {Code}", userId, code);
            }

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving forgot password record for user {UserID}", userId);
            throw;
        }
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var forgotPassword = await this.authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

            if (forgotPassword is null)
            {
                this.logger.LogWarning("Forgot password record {ID} not found for invalidation", id);
                return;
            }

            forgotPassword.IsActive = false;
            await this.authContext.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Successfully invalidated forgot password record {ID}", id);
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error invalidating forgot password record {ID}", id);
            throw;
        }
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            this.authContext.ForgottenPasswords.Add(forgotPassword);
            await this.authContext.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Successfully saved forgot password record for user {UserID}", forgotPassword.UserId);

            return forgotPassword;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error saving forgot password record for user {UserID}", forgotPassword.UserId);
            throw;
        }
    }
}
