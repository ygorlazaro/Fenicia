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

public class DataSourceService(
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

    public virtual async Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await customerService.GetAllForDataSourceAsync(cancellationToken);
    }

    public virtual async Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(CancellationToken cancellationToken = default)
    {
        return await employeeService.GetAllForDataSourceAsync(cancellationToken);
    }

    public virtual async Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(CancellationToken cancellationToken = default)
    {
        var positions = await positionService.GetAllAsync(new GetAllPositionQuery(1, int.MaxValue), cancellationToken);

        return [.. positions.Data.Select(p => p.MapToDataSourceResponse())];
    }

    public virtual async Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(CancellationToken cancellationToken = default)
    {
        var categories = await productCategoryService.GetAllAsync(new GetAllProductCategoryQuery(1, int.MaxValue), cancellationToken);

        return [.. categories.Data.Select(pc => pc.MapToDataSourceResponse())];
    }

    public virtual async Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        return await productService.GetAllForDataSourceAsync(cancellationToken);
    }

    public virtual async Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(CancellationToken cancellationToken = default)
    {
        return await supplierService.GetAllForDataSourceAsync(cancellationToken);
    }
}
