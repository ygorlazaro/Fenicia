using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.DataSource.DTOs;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Domains.DataSource;

public class DataSourceService(DefaultContext db)
{
    public async Task<List<GetAllCustomerForDataSourceResponse>> GetCustomersAsync(CancellationToken ct)
    {
        return await db.BasicCustomers.AsNoTracking().OrderBy(c => c.Person.Name).Select(c => new GetAllCustomerForDataSourceResponse(c.Id, c.Person.Name)).ToListAsync(ct);
    }

    public async Task<List<GetAllEmployeeForDataSourceResponse>> GetEmployeesAsync(CancellationToken ct)
    {
        return await db.BasicEmployees.AsNoTracking().OrderBy(e => e.Person.Name).Select(e => new GetAllEmployeeForDataSourceResponse(e.Id, e.Person.Name)).ToListAsync(ct);
    }

    public async Task<List<GetAllPositionForDataSourceResponse>> GetPositionsAsync(CancellationToken ct)
    {
        return await db.BasicPositions.OrderBy(p => p.Name).Select(p => new GetAllPositionForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }

    public async Task<List<GetAllProductCategoryForDataSourceResponse>> GetProductCategoriesAsync(CancellationToken ct)
    {
        return await db.BasicProductCategories.OrderBy(pc => pc.Name).Select(pc => new GetAllProductCategoryForDataSourceResponse(pc.Id, pc.Name)).ToListAsync(ct);
    }

    public async Task<List<GetAllProductForDataSourceResponse>> GetProductsAsync(CancellationToken ct)
    {
        return await db.BasicProducts.OrderBy(p => p.Name).Select(p => new GetAllProductForDataSourceResponse(p.Id, p.Name)).ToListAsync(ct);
    }

    public async Task<List<GetAllSupplierForDataSourceResponse>> GetSuppliersAsync(CancellationToken ct)
    {
        return await db.BasicSuppliers.OrderBy(s => s.Person.Name).Select(s => new GetAllSupplierForDataSourceResponse(s.Id, s.Person.Name)).ToListAsync(ct);
    }
}
