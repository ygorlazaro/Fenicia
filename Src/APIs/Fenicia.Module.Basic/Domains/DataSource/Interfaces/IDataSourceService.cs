using Fenicia.Module.Basic.Domains.DataSource.DTOs;

namespace Fenicia.Module.Basic.Domains.DataSource.Interfaces;

public interface IDataSourceService
{
    Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(CancellationToken cancellationToken = default);

    Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default);

    Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(CancellationToken cancellationToken = default);

    Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(CancellationToken cancellationToken = default);

    Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(CancellationToken cancellationToken = default);

    Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(CancellationToken cancellationToken = default);
}