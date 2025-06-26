namespace Fenicia.Auth.Domains.SubscriptionCredit.Logic;

using System.Diagnostics.CodeAnalysis;

using Common.Enums;

/// <summary>
///     Repository interface for managing subscription credit operations
/// </summary>
public interface ISubscriptionCreditRepository
{
    /// <summary>
    ///     Retrieves a list of valid module types based on the provided subscription IDs
    /// </summary>
    /// <param name="subscriptions">List of subscription GUIDs to validate</param>
    /// <param name="cancellationToken">Cancellation token to handle operation cancellation</param>
    /// <returns>A list of valid module types associated with the provided subscriptions</returns>
    /// <exception cref="OperationCanceledException">Thrown when the operation is canceled</exception>
    /// <exception cref="InvalidOperationException">Thrown when the operation cannot be completed</exception>
    [return: NotNull]
    Task<List<ModuleType>> GetValidModulesTypesAsync([NotNull] List<Guid> subscriptions, CancellationToken cancellationToken = default);
}
