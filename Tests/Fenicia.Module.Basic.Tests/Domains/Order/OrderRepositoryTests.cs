using Bogus;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order;
using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class OrderRepositoryTests : IDisposable
{
    private readonly DefaultContext _db;
    private readonly Faker _faker;
    private readonly OrderRepository _repository;

    public OrderRepositoryTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        var companyContext = new TestCompanyContext();
        _db = new DefaultContext(options, companyContext);
        _repository = new OrderRepository(_db);
        _faker = new Faker();
    }

    public void Dispose()
    {
        _db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllOrders()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetAllAsync(ct: CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.GetByIdAsync(order.Id, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task InsertAsync_WhenOrderIsValid_InsertsOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };

        var result = await _repository.InsertAsync(order, CancellationToken.None);

        Assert.NotNull(result);
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.NotEqual(default(DateTime), result.Created);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderExists_UpdatesOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.UpdateAsync(order.Id, order, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(order.Id, result.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };

        var result = await _repository.UpdateAsync(order.Id, order, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesOrder()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.DeleteAsync(order.Id, CancellationToken.None);

        Assert.Equal(1, result);
        var deletedOrder = await _db.BasicOrders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == order.Id);
        Assert.NotNull(deletedOrder);
        Assert.NotNull(deletedOrder.Deleted);
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.CountAsync(CancellationToken.None);

        Assert.Equal(1, result);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingOrders()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.FindAsync(o => o.Id == order.Id, CancellationToken.None);

        Assert.Single(result);
    }

    [Fact]
    public async Task AnyAsync_WhenOrderExists_ReturnsTrue()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.AnyAsync(o => o.Id == order.Id, CancellationToken.None);

        Assert.True(result);
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(), CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow, Status = OrderStatus.Pending };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        var result = await _repository.Query().Where(o => o.Id == order.Id).ToListAsync(CancellationToken.None);

        Assert.Single(result);
    }
}
