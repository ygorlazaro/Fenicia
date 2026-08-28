using Fenicia.Common.Data.Models.Basic;
using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Inventory;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;
using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Auth;
namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class GetOrderAnalyticsServiceTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly OrderService service;
    private readonly Guid companyId;

    public GetOrderAnalyticsServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        var orderRepository = new OrderRepository(db);
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var stockMovementRepository = new StockMovementRepository(db);
        var productRepository = new ProductRepository(db);
        service = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        faker = new Faker();
        companyId = companyContext.CompanyId;
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsOrderAnalytics()
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
