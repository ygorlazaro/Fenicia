namespace Fenicia.Auth.Domains.User;

using Common.Database.Contexts;

using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

public class UserRepository : IUserRepository
{
    private readonly AuthContext authContext;
    private readonly ILogger<UserRepository> logger;

    public UserRepository(AuthContext authContext, ILogger<UserRepository> logger)
    {
        this.authContext = authContext;
        this.logger = logger;
    }

    public async Task<UserModel?> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var result = await this.authContext.Users.FirstOrDefaultAsync(u => u.Email == email, cancellationToken);

            if (result == null)
            {
                this.logger.LogInformation("User not found for email: {Email}", email);
            }

            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving user for email: {Email}", email);
            throw;
        }
    }

    public UserModel Add(UserModel userRequest)
    {
        this.logger.LogInformation("Adding new user with email: {Email}", userRequest.Email);
        this.authContext.Users.Add(userRequest);
        return userRequest;
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await this.authContext.SaveChangesAsync(cancellationToken);
            this.logger.LogInformation("Successfully saved {Count} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error saving changes to database");
            throw;
        }
    }

    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await this.authContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
            this.logger.LogInformation("User existence check for email {Email}: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error checking user existence for email: {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await this.authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                this.logger.LogInformation("User not found for refresh token with ID: {UserID}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving user for refresh token with ID: {UserID}", userId);
            throw;
        }
    }

    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await this.authContext.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);

            if (userId == Guid.Empty)
            {
                this.logger.LogInformation("No user ID found for email: {Email}", email);
            }

            return userId;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving user ID for email: {Email}", email);
            throw;
        }
    }

    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await this.authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                this.logger.LogInformation("User not found with ID: {UserID}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, "Error retrieving user with ID: {UserID}", userId);
            throw;
        }
    }
}
