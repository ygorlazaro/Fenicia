using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class GetOrderAnalyticsServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;

    public GetOrderAnalyticsServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        service = new OrderService(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsAnalyticsResponse()
    {
        var result = await service.GetAnalyticsAsync(new GetOrderAnalyticsQuery(90, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotNull(result.OrdersByStatus);
        Assert.NotNull(result.SalesTrend);
        Assert.NotNull(result.TopCustomers);
        Assert.NotNull(result.AverageOrderValue);
        Assert.NotNull(result.CancelledOrders);
    }
}
