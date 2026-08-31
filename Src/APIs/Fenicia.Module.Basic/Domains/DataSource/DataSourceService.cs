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

public class DataSourceService
{
    private readonly CustomerService _customerService;
    private readonly EmployeeService _employeeService;
    private readonly PositionService _positionService;
    private readonly ProductCategoryService _productCategoryService;
    private readonly ProductService _productService;
    private readonly SupplierService _supplierService;

    public DataSourceService()
        : this(null!, null!, null!, null!, null!, null!)
    {
    }

    public DataSourceService(
        CustomerService customerService,
        EmployeeService employeeService,
        PositionService positionService,
        ProductCategoryService productCategoryService,
        ProductService productService,
        SupplierService supplierService)
    {
        _customerService = customerService;
        _employeeService = employeeService;
        _positionService = positionService;
        _productCategoryService = productCategoryService;
        _productService = productService;
        _supplierService = supplierService;
    }

    public virtual async Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(CancellationToken ct)
    {
        return await _customerService.GetAllForDataSourceAsync(ct);
    }

    public virtual async Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(CancellationToken ct)
    {
        return await _employeeService.GetAllForDataSourceAsync(ct);
    }

    public virtual async Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(CancellationToken ct)
    {
        var positions = await _positionService.GetAllAsync(new GetAllPositionQuery(1, int.MaxValue), ct);

        return [.. positions.Data.Select(p => p.MapToDataSourceResponse())];
    }

    public virtual async Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(CancellationToken ct)
    {
        var categories = await _productCategoryService.GetAllAsync(new GetAllProductCategoryQuery(1, int.MaxValue), ct);

        return [.. categories.Data.Select(pc => pc.MapToDataSourceResponse())];
    }

    public virtual async Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(CancellationToken ct)
    {
        return await _productService.GetAllForDataSourceAsync(ct);
    }

    public virtual async Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(CancellationToken ct)
    {
        return await _supplierService.GetAllForDataSourceAsync(ct);
    }
}
