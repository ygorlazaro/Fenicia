using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Customer;

namespace Fenicia.Module.Basic.Domains.Insight.Interfaces;

/// <summary>
/// Interface for insight-related operations.
/// </summary>
public interface IInsightService
{
    /// <summary>
    /// Gets customer insights based on the specified request.
    /// </summary>
    /// <param name="request">The customer insights request containing filter parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The customer insights response.</returns>
    Task<CustomerInsightsResponse> GetCustomerInsightsAsync(
        CustomerInsightsRequest request,
        CancellationToken cancellationToken = default);
}