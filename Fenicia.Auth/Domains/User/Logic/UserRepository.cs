namespace Fenicia.Auth.Domains.User.Logic;

using Contexts;

using Data;

using Microsoft.EntityFrameworkCore;

/// <summary>
///     Repository for managing user-related database operations
/// </summary>
public class UserRepository(AuthContext authContext, ILogger<UserRepository> logger) : IUserRepository
{
    /// <summary>
    ///     Retrieves a user by email and company CNPJ
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="cnpj">Company's CNPJ</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User model if found, null otherwise</returns>
    public async Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj, CancellationToken cancellationToken)
    {
        try
        {
            var query = from user in authContext.Users join userRole in authContext.UserRoles on user.Id equals userRole.UserId join company in authContext.Companies on userRole.CompanyId equals company.Id where user.Email == email && company.Cnpj == cnpj select user;

            var result = await query.FirstOrDefaultAsync(cancellationToken);

            if (result == null)
            {
                logger.LogInformation(message: "User not found for email: {Email} and CNPJ: {Cnpj}", email, cnpj);
            }

            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving user for email: {Email} and CNPJ: {Cnpj}", email, cnpj);
            throw;
        }
    }

    /// <summary>
    ///     Adds a new user to the database context
    /// </summary>
    /// <param name="userRequest">User model to add</param>
    /// <returns>Added user model</returns>
    public UserModel Add(UserModel userRequest)
    {
        logger.LogInformation(message: "Adding new user with email: {Email}", userRequest.Email);
        authContext.Users.Add(userRequest);
        return userRequest;
    }

    /// <summary>
    ///     Saves changes to the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    public async Task<int> SaveAsync(CancellationToken cancellationToken)
    {
        try
        {
            var result = await authContext.SaveChangesAsync(cancellationToken);
            logger.LogInformation(message: "Successfully saved {Count} changes to database", result);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error saving changes to database");
            throw;
        }
    }

    /// <summary>
    ///     Checks if a user with the specified email exists
    /// </summary>
    /// <param name="email">Email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists, false otherwise</returns>
    public async Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var exists = await authContext.Users.AnyAsync(u => u.Email == email, cancellationToken);
            logger.LogInformation(message: "User existence check for email {Email}: {Exists}", email, exists);
            return exists;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error checking user existence for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    ///     Retrieves a user for refresh token validation
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User model if found, null otherwise</returns>
    public async Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                logger.LogInformation(message: "User not found for refresh token with ID: {UserId}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving user for refresh token with ID: {UserId}", userId);
            throw;
        }
    }

    /// <summary>
    ///     Retrieves a user's ID by their email address
    /// </summary>
    /// <param name="email">User's email address</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User's ID if found, null otherwise</returns>
    public async Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken)
    {
        try
        {
            var userId = await authContext.Users.Where(u => u.Email == email).Select(u => u.Id).FirstOrDefaultAsync(cancellationToken);
            if (userId == Guid.Empty)
            {
                logger.LogInformation(message: "No user ID found for email: {Email}", email);
            }

            return userId;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving user ID for email: {Email}", email);
            throw;
        }
    }

    /// <summary>
    ///     Retrieves a user by their ID
    /// </summary>
    /// <param name="userId">User's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User model if found, null otherwise</returns>
    public async Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken)
    {
        try
        {
            var user = await authContext.Users.FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            if (user == null)
            {
                logger.LogInformation(message: "User not found with ID: {UserId}", userId);
            }

            return user;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error retrieving user with ID: {UserId}", userId);
            throw;
        }
    }
}
