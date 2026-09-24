using Fenicia.Common.DTOs.Basic.DataSource;
using Fenicia.Module.Basic.Domains.Customer.Interfaces;
using Fenicia.Module.Basic.Domains.DataSource.Interfaces;
using Fenicia.Module.Basic.Domains.Employee.Interfaces;
using Fenicia.Module.Basic.Domains.Position.Interfaces;
using Fenicia.Module.Basic.Domains.Product.Interfaces;
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

    public Task<List<DashboardProductDataSourceResponse>> GetDashboardProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return productService.GetAllDashboardForDataSourceAsync(cancellationToken);
    }

    public Task<List<CustomerDataSourceResponse>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        return customerService.GetAllForDataSourceAsync(cancellationToken);
    }

    public Task<List<EmployeeDataSourceResponse>> GetEmployeesAsync(
        CancellationToken cancellationToken = default)
    {
        return employeeService.GetAllForDataSourceAsync(cancellationToken);
    }

    public async Task<List<PositionDataSourceResponse>> GetPositionsAsync(
        CancellationToken cancellationToken = default)
    {
        var positions = await positionService.GetAllAsync(1, int.MaxValue, cancellationToken);

        return [.. positions.Data.Select(p => new PositionDataSourceResponse(p.Id, p.Name))];
    }

    public async Task<List<ProductCategoryDataSourceResponse>> GetProductCategoriesAsync(
        CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAllAsync(1, int.MaxValue, cancellationToken);

        return [.. categories.Data.Select(c => new ProductCategoryDataSourceResponse(c.Id, c.Name))];
    }

    public Task<List<ProductDataSourceResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        return productService.GetAllForDataSourceAsync(cancellationToken);
    }

    public Task<List<SupplierDataSourceResponse>> GetSuppliersAsync(
        CancellationToken cancellationToken = default)
    {
        return supplierService.GetAllForDataSourceAsync(cancellationToken);
    }
}