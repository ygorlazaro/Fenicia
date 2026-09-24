using Fenicia.Common.DTOs.Basic.DataSource;

namespace Fenicia.Module.Basic.Domains.DataSource.Interfaces;

/// <summary>
/// Interface for data source service operations (dropdowns, lookups).
/// </summary>
public interface IDataSourceService
{
    /// <summary>
    /// Gets the list of customers for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of customer data source responses.</returns>
    Task<List<CustomerDataSourceResponse>> GetCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of employees for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of employee data source responses.</returns>
    Task<List<EmployeeDataSourceResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of positions for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of position data source responses.</returns>
    Task<List<PositionDataSourceResponse>> GetPositionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of product categories for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product category data source responses.</returns>
    Task<List<ProductCategoryDataSourceResponse>> GetProductCategoriesAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of products for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of product data source responses.</returns>
    Task<List<ProductDataSourceResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of dashboard products for data source (POS, sales).
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of dashboard product data source responses.</returns>
    Task<List<DashboardProductDataSourceResponse>> GetDashboardProductsAsync(
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the list of suppliers for data source.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
    /// <returns>A list of supplier data source responses.</returns>
    Task<List<SupplierDataSourceResponse>> GetSuppliersAsync(
        CancellationToken cancellationToken = default);
}