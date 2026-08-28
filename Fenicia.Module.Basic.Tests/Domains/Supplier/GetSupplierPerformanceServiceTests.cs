using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Supplier.DTOs;
using Fenicia.Module.Basic.Domains.Supplier.Services;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Supplier;

public class GetSupplierPerformanceServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly SupplierService service;

    public GetSupplierPerformanceServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new SupplierService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetPerformanceAsync_ReturnsPerformanceResponse()
    {
        var result = await service.GetPerformanceAsync(new GetSupplierPerformanceQuery(90, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.ProductsPerSupplier);
        Assert.NotNull(result.CostComparison);
        Assert.NotNull(result.RecentStockMovements);
        Assert.NotNull(result.Summary);
    }
}
