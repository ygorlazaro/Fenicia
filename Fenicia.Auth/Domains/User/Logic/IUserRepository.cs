using Fenicia.Auth.Domains.User.Data;

namespace Fenicia.Auth.Domains.User.Logic;

/// <summary>
/// Repository interface for managing user-related data operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Retrieves a user by their email and CNPJ
    /// </summary>
    /// <param name="email">The user's email address</param>
    /// <param name="cnpj">The company's CNPJ</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>UserModel if found, null otherwise</returns>
    Task<UserModel?> GetByEmailAndCnpjAsync(string email, string cnpj, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a new user to the repository
    /// </summary>
    /// <param name="userRequest">The user model to be added</param>
    /// <returns>The added user model</returns>
    UserModel Add(UserModel userRequest);

    /// <summary>
    /// Saves changes to the repository
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Number of affected records</returns>
    Task<int> SaveAsync(CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user exists by email
    /// </summary>
    /// <param name="email">The email to check</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if user exists, false otherwise</returns>
    Task<bool> CheckUserExistsAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user for refresh token validation
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>UserModel if found, null otherwise</returns>
    Task<UserModel?> GetUserForRefreshTokenAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Gets user ID from email address
    /// </summary>
    /// <param name="email">The email address to lookup</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>User ID if found, null otherwise</returns>
    Task<Guid?> GetUserIdFromEmailAsync(string email, CancellationToken cancellationToken);

    /// <summary>
    /// Retrieves a user by their ID
    /// </summary>
    /// <param name="userId">The user's unique identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>UserModel if found, null otherwise</returns>
    Task<UserModel?> GetByIdAsync(Guid userId, CancellationToken cancellationToken);
}
