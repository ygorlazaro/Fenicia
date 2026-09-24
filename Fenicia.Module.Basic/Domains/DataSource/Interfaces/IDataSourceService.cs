using Fenicia.Common.DTOs.Basic.DataSource;

namespace Fenicia.Module.Basic.Domains.DataSource.Interfaces;

public interface IDataSourceService
{
    Task<List<CustomerDataSourceResponse>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<List<EmployeeDataSourceResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    Task<List<PositionDataSourceResponse>> GetPositionsAsync(CancellationToken cancellationToken = default);

    Task<List<ProductCategoryDataSourceResponse>> GetProductCategoriesAsync(
        CancellationToken cancellationToken = default);

    Task<List<ProductDataSourceResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default);

    Task<List<DashboardProductDataSourceResponse>> GetDashboardProductsAsync(
        CancellationToken cancellationToken = default);

    Task<List<SupplierDataSourceResponse>> GetSuppliersAsync(
        CancellationToken cancellationToken = default);
}