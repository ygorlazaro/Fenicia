using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.DTOs.Queries;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

public class GetInventoryDashboardServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly InventoryService service;

    public GetInventoryDashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new InventoryService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsDashboardResponse()
    {
        var result = await service.GetDashboardAsync(new GetInventoryDashboardQuery(), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.LowStockItems);
        Assert.NotNull(result.CategoryBreakdown);
        Assert.NotNull(result.SupplierBreakdown);
    }
}
