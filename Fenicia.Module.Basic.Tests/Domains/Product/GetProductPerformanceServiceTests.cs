using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.Product.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductPerformanceServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly ProductService service;

    public GetProductPerformanceServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new ProductService(db);
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
        var result = await service.GetPerformanceAsync(new GetProductPerformanceQuery(90, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.BestSellingProducts);
        Assert.NotNull(result.WorstSellingProducts);
        Assert.NotNull(result.ProfitMargins);
        Assert.NotNull(result.NeverSoldProducts);
    }
}
