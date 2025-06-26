using Fenicia.Auth.Domains.Role.Data;

namespace Fenicia.Auth.Domains.Role.Logic;

/// <summary>
/// Interface for managing role-related database operations
/// </summary>
public interface IRoleRepository
{
    /// <summary>
    /// Retrieves the Admin role from the database
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation</param>
    /// <returns>Returns the Admin role model if found, null otherwise</returns>
    /// <remarks>
    /// This method specifically looks for a role with the name "Admin"
    /// </remarks>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled</exception>
    /// <exception cref="InvalidOperationException">Thrown when there's an error accessing the database</exception>
    Task<RoleModel?> GetAdminRoleAsync(CancellationToken cancellationToken);
}
