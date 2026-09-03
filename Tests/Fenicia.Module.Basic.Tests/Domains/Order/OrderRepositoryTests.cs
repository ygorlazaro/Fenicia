using AwesomeAssertions;
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
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
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
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetAllAsync(cancellationToken: CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.GetByIdAsync(order.Id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _repository.GetByIdAsync(Guid.NewGuid(), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task InsertAsync_WhenOrderIsValid_InsertsOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        // Act
        var result = await _repository.InsertAsync(order, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
        result.Created.Should().NotBe(default);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderExists_UpdatesOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.UpdateAsync(order.Id, order, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task UpdateAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        // Act
        var result = await _repository.UpdateAsync(order.Id, order, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.DeleteAsync(order.Id, CancellationToken.None);

        // Assert
        result.Should().Be(1);
        var deletedOrder = await _db.BasicOrders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == order.Id);
        deletedOrder.Should().NotBeNull();
        deletedOrder.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task CountAsync_ReturnsCorrectCount()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.CountAsync(CancellationToken.None);

        // Assert
        result.Should().Be(1);
    }

    [Fact]
    public async Task FindAsync_ReturnsMatchingOrders()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.FindAsync(o => o.Id == order.Id, CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }

    [Fact]
    public async Task AnyAsync_WhenOrderExists_ReturnsTrue()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.AnyAsync(o => o.Id == order.Id, CancellationToken.None);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task Query_ReturnsQueryable()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(), OrderNumber = _faker.Random.AlphaNumeric(10), UserId = Guid.NewGuid(),
            CustomerId = Guid.NewGuid(), TotalAmount = _faker.Random.Decimal(), SaleDate = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _repository.Query().Where(o => o.Id == order.Id).ToListAsync(CancellationToken.None);

        // Assert
        result.Should().HaveCount(1);
    }
}