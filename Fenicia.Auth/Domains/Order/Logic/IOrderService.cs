namespace Fenicia.Auth.Domains.Order.Logic;

using Common;

using Data;

/// <summary>
///     Provides order management operations and business logic for the application.
/// </summary>
public interface IOrderService
{
    /// <summary>
    ///     Creates a new order asynchronously.
    /// </summary>
    /// <param name="userId">The unique identifier of the user creating the order.</param>
    /// <param name="companyId">The unique identifier of the company associated with the order.</param>
    /// <param name="request">The order request containing order details.</param>
    /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
    /// <returns>An API response containing the created order response.</returns>
    Task<ApiResponse<OrderResponse>> CreateNewOrderAsync(Guid userId, Guid companyId, OrderRequest request, CancellationToken cancellationToken);
}
