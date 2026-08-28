using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product;
using Fenicia.Module.Basic.Domains.StockMovement.DTOs;
using Fenicia.Module.Basic.Domains.StockMovement;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.History);
        Assert.NotNull(result.MonthlyInOut);
        Assert.NotNull(result.TopMovedProducts);
        Assert.NotNull(result.TurnoverRates);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;
    private Guid companyId;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly StockMovementService service;
    public async Task GetDashboardAsync_ReturnsDashboardResponse()
public class GetStockMovementDashboardServiceTests : IDisposable
    public GetStockMovementDashboardServiceTests()
    public void Dispose()
        service = new StockMovementService(stockMovementRepository, productRepository);
        var companyContext = new TestCompanyContext();
        var companyId = companyContext.CompanyId;
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var productRepository = new ProductRepository(db);
        var result = await service.GetDashboardAsync(new GetStockMovementDashboardQuery(30, 10), CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);
