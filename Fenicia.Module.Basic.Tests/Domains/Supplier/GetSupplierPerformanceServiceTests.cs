using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Fenicia.Module.Basic.Domains.Supplier;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.CostComparison);
        Assert.NotNull(result.ProductsPerSupplier);
        Assert.NotNull(result.RecentStockMovements);
        Assert.NotNull(result.Summary);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Supplier;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;
    public async Task GetPerformanceAsync_ReturnsPerformanceResponse()
public class GetSupplierPerformanceServiceTests : IDisposable
    public GetSupplierPerformanceServiceTests()
    public void Dispose()
        service = new SupplierService(supplierRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await service.GetPerformanceAsync(new GetSupplierPerformanceQuery(90, 10), CancellationToken.None);
        var supplierRepository = new SupplierRepository(db);
