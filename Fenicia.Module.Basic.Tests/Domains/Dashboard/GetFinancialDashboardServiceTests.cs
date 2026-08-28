using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Dashboard;
using Fenicia.Module.Basic.Domains.Dashboard.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Dashboard;

public class GetFinancialDashboardServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly DashboardService service;

    public GetFinancialDashboardServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var dashboardRepository = new DashboardRepository(db);
        service = new DashboardService(dashboardRepository);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetFinancialDashboardAsync_ReturnsFinancialDashboardResponse()
    {
        var result = await service.GetFinancialDashboardAsync(new GetFinancialDashboardQuery(90), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.Kpi);
        Assert.NotNull(result.RevenueVsCost);
        Assert.NotNull(result.ProfitMarginTrend);
        Assert.NotNull(result.AccountsReceivable);
        Assert.NotNull(result.DailySales);
    }
}
