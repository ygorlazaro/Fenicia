using Fenicia.Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.User;

public class UserRepository(AuthContext context, ILogger<UserRepository> logger) : IUserRepository
{
    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var result = await context.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (result == null)
            {
                logger.LogInformation("User not found for email: {Email}", email);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user for email: {Email}", email);

            throw;
        }
    }

    public UserModel Add(UserModel userRequest)
    {
        logger.LogInformation("Adding new user with email: {Email}", userRequest.Email);

        context.Users.Add(userRequest);

        return userRequest;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await context.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Successfully saved {Count} changes to database", result);

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving changes to database");

            throw;
        }
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await context.Users.AnyAsync(u => u.Email == email, cancellationToken);

            logger.LogInformation("User existence check for email {Email}: {Exists}", email, exists);

            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error checking user existence for email: {Email}", email);

            throw;
        }
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                logger.LogInformation("User not found for refresh token with ID: {UserID}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user for refresh token with ID: {UserID}", userId);

            throw;
        }
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await context.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);

            if (userId == Guid.Empty)
            {
                logger.LogInformation("No user ID found for email: {Email}", email);
            }

            return userId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user ID for email: {Email}", email);

            throw;
        }
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await context.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);

            if (user == null)
            {
                logger.LogInformation("User not found with ID: {UserID}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error retrieving user with ID: {UserID}", userId);

            throw;
        }
    }
}
