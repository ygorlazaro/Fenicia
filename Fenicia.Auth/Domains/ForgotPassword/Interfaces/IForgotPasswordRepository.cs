using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;

namespace Fenicia.Auth.Domains.ForgotPassword.Interfaces;

/// <summary>
/// Repository interface for managing forgot password operations, including retrieval of active forgot password records by user ID and code.
/// </summary>
public interface IForgotPasswordRepository : IRepository<ForgotPasswordModel>
{
    /// <summary>
    /// Retrieves an active forgot password record based on the provided user ID and code. Returns null if no active record is found.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="code">The forgot password code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The forgot password model or null if not found.</returns>
    Task<ForgotPasswordModel?> GetActiveByUserIdAndCodeAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default);
}
