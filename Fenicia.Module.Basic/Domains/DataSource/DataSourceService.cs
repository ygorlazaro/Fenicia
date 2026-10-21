using Fenicia.Common;
using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Common.DTOs.Basic.Position;
using Fenicia.Common.DTOs.Basic.ProductCategory;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Fenicia.Module.Basic.Domains.ProductCategory.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
using Fenicia.Module.Basic.Domains.Supplier.Interfaces;

namespace Fenicia.Module.Basic.Domains.DataSource;

/// <summary>
/// Service implementation for managing data source operations (dropdowns, lookups).
/// </summary>
/// <param name="customerService">The customer service.</param>
/// <param name="employeeService">The employee service.</param>
/// <param name="positionService">The position service.</param>
/// <param name="productCategoryService">The product category service.</param>
/// <param name="productService">The product service.</param>
/// <param name="supplierService">The supplier service.</param>
public sealed class DataSourceService(
    ICustomerService customerService,
    IEmployeeService employeeService,
    IPositionService positionService,
    IProductCategoryService productCategoryService,
    IProductService productService,
    ISupplierService supplierService) : IDataSourceService
{
    /// <inheritdoc />
    public Task<List<DashboardProductDataSourceResponse>> GetDashboardProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return productService.GetAllDashboardForDataSourceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<CustomerDataSourceResponse>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return customerService.GetAllForDataSourceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<EmployeeDataSourceResponse>> GetEmployeesAsync(
        CancellationToken cancellationToken = default)
    {
        return employeeService.GetAllForDataSourceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public async Task<List<PositionDataSourceResponse>> GetPositionsAsync(
        CancellationToken cancellationToken = default)
    {
        var positions = await positionService.GetAllAsync(new PositionQuery(1, int.MaxValue), cancellationToken);

        return [.. positions.Data.Select(p => new PositionDataSourceResponse(p.Id, p.Name))];
    }

    /// <inheritdoc />
    public async Task<List<ProductCategoryDataSourceResponse>> GetProductCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAllAsync(new ProductCategoryQuery(1, int.MaxValue), cancellationToken);

        return [.. categories.Data.Select(c => new ProductCategoryDataSourceResponse(c.Id, c.Name))];
    }

    /// <inheritdoc />
    public Task<List<ProductDataSourceResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return productService.GetAllForDataSourceAsync(cancellationToken);
    }

    /// <inheritdoc />
    public Task<List<SupplierDataSourceResponse>> GetSuppliersAsync(
        CancellationToken cancellationToken = default)
    {
        return supplierService.GetAllForDataSourceAsync(cancellationToken);
    }
}
