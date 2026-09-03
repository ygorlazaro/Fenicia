using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;

namespace Fenicia.Module.Basic.Domains.DataSource;

public sealed class DataSourceService(
    ICustomerService customerService,
    IEmployeeService employeeService,
    IPositionService positionService,
    IProductCategoryService productCategoryService,
    IProductService productService,
    ISupplierService supplierService) : IDataSourceService
{
    public DataSourceService()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return customerService.GetAllForDataSourceAsync(cancellationToken);
    }

    public Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(
        CancellationToken cancellationToken = default)
    {
        return employeeService.GetAllForDataSourceAsync(cancellationToken);
    }

    public async Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(
        CancellationToken cancellationToken = default)
    {
        var positions = await positionService.GetAllAsync(new GetAllPositionQuery(1, int.MaxValue), cancellationToken);

        return [.. positions.Data.Select(p => p.MapToDataSourceResponse())];
    }

    public async Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAllAsync(
            new GetAllProductCategoryQuery(1, int.MaxValue),
            cancellationToken);

        return [.. categories.Data.Select(pc => pc.MapToDataSourceResponse())];
    }

    public Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return productService.GetAllForDataSourceAsync(cancellationToken);
    }

    public Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(
        CancellationToken cancellationToken = default)
    {
        return supplierService.GetAllForDataSourceAsync(cancellationToken);
    }
}