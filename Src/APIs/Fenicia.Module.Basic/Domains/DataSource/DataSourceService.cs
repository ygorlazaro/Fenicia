using Fenicia.Module.Basic.Domains.Customer;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;
using Fenicia.Module.Basic.Domains.Employee;
using Fenicia.Module.Basic.Domains.Position;
using Fenicia.Module.Basic.Domains.Position.DTOs;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.ProductCategory;
using Fenicia.Module.Basic.Domains.ProductCategory.DTOs;
using Fenicia.Module.Basic.Domains.Supplier;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class DataSourceService(
    CustomerService customerService,
    EmployeeService employeeService,
    PositionService positionService,
    ProductCategoryService productCategoryService,
    ProductService productService,
    SupplierService supplierService)
{
    public async Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(CancellationToken ct)
    {
        return await customerService.GetAllForDataSourceAsync(ct);
    }

    public async Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(CancellationToken ct)
    {
        return await employeeService.GetAllForDataSourceAsync(ct);
    }

    public async Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(CancellationToken ct)
    {
        var positions = await positionService.GetAllAsync(new GetAllPositionQuery(1, int.MaxValue), ct);

        return positions.Data.Select(p => p.MapToDataSourceResponse()).ToList();
    }

    public async Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(CancellationToken ct)
    {
        var categories = await productCategoryService.GetAllAsync(new GetAllProductCategoryQuery(1, int.MaxValue), ct);

        return categories.Data.Select(pc => pc.MapToDataSourceResponse()).ToList();
    }

    public async Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(CancellationToken ct)
    {
        return await productService.GetAllForDataSourceAsync(ct);
    }

    public async Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(CancellationToken ct)
    {
        return await supplierService.GetAllForDataSourceAsync(ct);
    }
}
