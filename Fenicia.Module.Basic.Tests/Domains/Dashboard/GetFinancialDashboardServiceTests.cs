using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Fenicia.Module.Basic.Domains.Dashboard;
using Microsoft.EntityFrameworkCore;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.AccountsReceivable);
        Assert.NotNull(result.DailySales);
        Assert.NotNull(result.Kpi);
        Assert.NotNull(result.ProfitMarginTrend);
        Assert.NotNull(result.RevenueVsCost);
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;
    private readonly DashboardService service;
    private readonly DefaultContext db;
    private readonly Faker faker;
    public async Task GetFinancialDashboardAsync_ReturnsFinancialDashboardResponse()
public class GetFinancialDashboardServiceTests : IDisposable
    public GetFinancialDashboardServiceTests()
    public void Dispose()
        service = new DashboardService(dashboardRepository);
        var companyContext = new TestCompanyContext();
        var dashboardRepository = new DashboardRepository(db);
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var result = await service.GetFinancialDashboardAsync(new GetFinancialDashboardQuery(90), CancellationToken.None);
