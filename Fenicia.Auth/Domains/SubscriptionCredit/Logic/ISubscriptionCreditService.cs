using Fenicia.Common;
using Fenicia.Common.Enums;

namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

/// <summary>
/// Service responsible for managing subscription credits and module access.
/// </summary>
public interface ISubscriptionCreditService
{
    /// <summary>
    /// Retrieves a list of active module types for a specified company.
    /// </summary>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
    /// <returns>An ApiResponse containing a list of active ModuleType entities. Returns an empty list if no active modules are found.</returns>
    /// <exception cref="ArgumentException">Thrown when companyId is empty.</exception>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled.</exception>
    Task<ApiResponse<List<ModuleType>>> GetActiveModulesTypesAsync(Guid companyId,
        CancellationToken cancellationToken);
}
