using Fenicia.Auth.Domains.ForgotPassword.Interfaces;
using Fenicia.Common.Data.Models.Auth;
using Fenicia.Common.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Domains.ForgotPassword;

/// <summary>
/// Repository implementation for managing forgot password operations, including retrieval of active forgot password records by user ID and code.
/// </summary>
/// <param name="context"></param>
public class ForgotPasswordRepository(DbContext context)
    : Repository<ForgotPasswordModel>(context), IForgotPasswordRepository
{
    /// <summary>
    /// Retrieves an active forgot password record based on the provided user ID and code. Returns null if no active record is found.
    /// </summary>
    /// <param name="userId">The user ID.</param>
    /// <param name="code">The forgot password code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The forgot password model or null if not found.</returns>
    public Task<ForgotPasswordModel?> GetActiveByUserIdAndCodeAsync(
        Guid userId,
        string code,
        CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var query = from fp in DbSet
                    where fp.UserId == userId
                        && fp.Code == code
                        && fp.IsActive
                        && fp.ExpirationDate >= now
                    select fp;

        return query.FirstOrDefaultAsync(cancellationToken);
    }
}
