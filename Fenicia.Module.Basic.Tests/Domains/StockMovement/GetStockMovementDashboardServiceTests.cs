using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

public class GetStockMovementDashboardServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StockMovementService service;

    public GetStockMovementDashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new StockMovementService(db);
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
        var result = await service.GetDashboardAsync(new GetStockMovementDashboardQuery(30, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.History);
        Assert.NotNull(result.MonthlyInOut);
        Assert.NotNull(result.TopMovedProducts);
        Assert.NotNull(result.TurnoverRates);
    }
}
