using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.Order;
using Microsoft.EntityFrameworkCore;
using SalesOrderDetailRepository = Fenicia.Module.Basic.Domains.OrderDetail.OrderDetailRepository;

    {
    }
{
}
        Assert.NotNull(result);
        Assert.NotNull(result.AverageOrderValue);
        Assert.NotNull(result.CancelledOrders);
        Assert.NotNull(result.OrdersByStatus);
        Assert.NotNull(result.SalesTrend);
        Assert.NotNull(result.TopCustomers);
        companyId = companyContext.CompanyId;
        db.Dispose();
        db = new DefaultContext(options, companyContext);
    [Fact]
        faker = new Faker();
        GC.SuppressFinalize(this);
namespace Fenicia.Module.Basic.Tests.Domains.Order;
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly Guid companyId;
    private readonly OrderService service;
    public async Task GetAnalyticsAsync_ReturnsOrderAnalytics()
public class GetOrderAnalyticsServiceTests : IDisposable
    public GetOrderAnalyticsServiceTests()
    public void Dispose()
        service = new OrderService(orderRepository, orderDetailRepository, stockMovementRepository, productRepository);
        var companyContext = new TestCompanyContext();
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var orderDetailRepository = new SalesOrderDetailRepository(db);
        var orderRepository = new OrderRepository(db);
        var productRepository = new ProductRepository(db);
        var result = await service.GetAnalyticsAsync(new GetOrderAnalyticsQuery(90, 10), CancellationToken.None);
        var stockMovementRepository = new StockMovementRepository(db);
