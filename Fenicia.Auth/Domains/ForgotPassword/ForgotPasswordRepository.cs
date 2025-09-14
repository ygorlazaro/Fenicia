namespace Fenicia.Auth.Domains.ForgotPassword;

using Common.Database.Contexts;
using Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class ForgotPasswordRepository : IForgotPasswordRepository
{
    private readonly AuthContext _authContext;
    private readonly ILogger<ForgotPasswordRepository> _logger;

    public ForgotPasswordRepository(AuthContext authContext, ILogger<ForgotPasswordRepository> logger)
    {
        _authContext = authContext;
        _logger = logger;
    }

    public async Task<ForgotPasswordModel?> GetFromUserIdAndCodeAsync(Guid userId, string code, CancellationToken cancellationToken)
    {
        try
        {
            var now = DateTime.UtcNow;
            var query = from forgotPassword in _authContext.ForgottenPasswords where forgotPassword.UserId == userId && forgotPassword.Code == code && forgotPassword.IsActive && forgotPassword.ExpirationDate >= now select forgotPassword;

            var result = await query.FirstOrDefaultAsync(cancellationToken);
            if (result == null)
            {
                _logger.LogWarning("No active forgot password record found for user {UserId} with code {Code}", userId, code);
            }

            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving forgot password record for user {UserId}", userId);
            throw;
        }
    }

    public async Task InvalidateCodeAsync(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            var forgotPassword = await _authContext.ForgottenPasswords.FirstOrDefaultAsync(fp => fp.Id == id, cancellationToken);

            if (forgotPassword is null)
            {
                _logger.LogWarning("Forgot password record {Id} not found for invalidation", id);
                return;
            }

            forgotPassword.IsActive = false;
            await _authContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully invalidated forgot password record {Id}", id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error invalidating forgot password record {Id}", id);
            throw;
        }
    }

    public async Task<ForgotPasswordModel> SaveForgotPasswordAsync(ForgotPasswordModel forgotPassword, CancellationToken cancellationToken)
    {
        try
        {
            _authContext.ForgottenPasswords.Add(forgotPassword);
            await _authContext.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Successfully saved forgot password record for user {UserId}", forgotPassword.UserId);

            return forgotPassword;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error saving forgot password record for user {UserId}", forgotPassword.UserId);
            throw;
        }
    }
}
