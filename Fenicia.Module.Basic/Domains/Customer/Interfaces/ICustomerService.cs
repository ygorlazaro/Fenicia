using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.Customer;
using Fenicia.Common.DTOs.Basic.DataSource;

namespace Fenicia.Module.Basic.Domains.Customer.Interfaces;

/// <summary>
/// Interface for customer service operations.
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Gets a paginated list of all customers.
    /// </summary>
    /// <param name="query">The pagination query parameters.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A paginated list of customer responses.</returns>
    Task<Pagination<List<CustomerResponse>>> GetAllAsync(
        PaginationQuery query,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all customers for data source (dropdown/lookup) scenarios.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of customer data source responses.</returns>
    Task<List<CustomerDataSourceResponse>> GetAllForDataSourceAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a customer by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The customer response if found; otherwise, null.</returns>
    Task<CustomerResponse?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Adds a new customer.
    /// </summary>
    /// <param name="request">The customer request data.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The created customer response.</returns>
    Task<CustomerResponse> AddAsync(
        CustomerRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing customer.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="request">The updated customer request data.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The updated customer response if found; otherwise, null.</returns>
    Task<CustomerResponse?> UpdateAsync(
        Guid id,
        CustomerRequest request,
        Guid companyId,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a customer by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the customer.</param>
    /// <param name="companyId">The unique identifier of the company.</param>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteAsync(Guid id, Guid companyId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of customers.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>The total number of customers.</returns>
    Task<int> GetCountAsync(CancellationToken cancellationToken = default);
}