namespace Fenicia.Auth.Domains.UserRole.Logic;

/// <summary>
/// Represents a repository for managing user roles.
/// </summary>
public interface IUserRoleRepository
{
    /// <summary>
    /// Retrieves all roles associated with a specific user.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>An array of role names assigned to the user.</returns>
    /// <exception cref="ArgumentException">Thrown when userId is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the operation fails.</exception>
    Task<string[]> GetRolesByUserAsync(Guid userId, CancellationToken cancellationToken);

    /// <summary>
    /// Checks if a user exists within a specific company.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>True if the user exists in the company; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when userId or companyId is empty.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the operation fails.</exception>
    Task<bool> ExistsInCompanyAsync(Guid userId, Guid companyId, CancellationToken cancellationToken);

    /// <summary>
    /// Verifies if a user has a specific role within a company.
    /// </summary>
    /// <param name="guid">The unique identifier of the user.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="role">The role name to check.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>True if the user has the specified role; otherwise, false.</returns>
    /// <exception cref="ArgumentException">Thrown when any parameter is invalid.</exception>
    /// <exception cref="InvalidOperationException">Thrown when the operation fails.</exception>
    Task<bool> HasRoleAsync(Guid guid, Guid companyId, string role, CancellationToken cancellationToken);
}
