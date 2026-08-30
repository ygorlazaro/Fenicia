using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Fenicia.Module.Basic.Domains.Order.DTOs;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.StockMovement;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderServiceTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderService _service;

    public OrderServiceTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        var repository = new OrderRepository(_db);
        _service = new OrderService(repository, new OrderDetailService(new OrderDetailRepository(_db)), new StockMovementService());
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_WhenOrdersExist_ReturnsPaginationWithOrders()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), SaleDate = DateTime.UtcNow, TotalAmount = _faker.Random.Decimal() };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetAllAsync(new GetAllOrderQuery(1, 10), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), SaleDate = DateTime.UtcNow, TotalAmount = _faker.Random.Decimal() };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesOrder()
    {
        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, OrderStatus.Pending, [], PaymentMethod.Cash);

        var result = await _service.CreateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), SaleDate = DateTime.UtcNow, TotalAmount = _faker.Random.Decimal() };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        await _service.DeleteAsync(new DeleteOrderCommand(order.Id), Guid.NewGuid(), CancellationToken.None);

        var deletedOrder = await _db.BasicOrders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == order.Id);
        Assert.NotNull(deletedOrder);
        Assert.NotNull(deletedOrder.Deleted);
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsAnalytics()
    {
        var result = await _service.GetAnalyticsAsync(new GetOrderAnalyticsQuery(90, 10), CancellationToken.None);

        Assert.NotNull(result);
    }
}
