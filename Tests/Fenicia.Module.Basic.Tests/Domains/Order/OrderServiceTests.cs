using AwesomeAssertions;
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
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id };
        _db.BasicCustomers.Add(customer);
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), SaleDate = DateTime.UtcNow, TotalAmount = _faker.Random.Decimal(), CustomerId = customer.Id };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetAllAsync(new GetAllOrderQuery(1, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderExists_ReturnsOrder()
    {
        // Arrange
        var person = new PersonModel { Id = Guid.NewGuid(), Name = _faker.Person.FullName };
        _db.BasicPeople.Add(person);
        var customer = new CustomerModel { Id = Guid.NewGuid(), PersonId = person.Id };
        _db.BasicCustomers.Add(customer);
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), UserId = Guid.NewGuid(), CustomerId = customer.Id, TotalAmount = _faker.Random.Decimal(), DiscountAmount = _faker.Random.Decimal(), TotalQuantity = _faker.Random.Int(), SaleDate = _faker.Date.Recent(), Status = OrderStatus.Pending, PaymentMethod = PaymentMethod.Cash };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(order.Id), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
    }

    [Fact]
    public async Task GetByIdAsync_WhenOrderDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _service.GetByIdAsync(new GetOrderByIdQuery(Guid.NewGuid()), CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_WhenCommandIsValid_CreatesOrder()
    {
        // Arrange
        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, OrderStatus.Pending, [], PaymentMethod.Cash);

        // Act
        var result = await _service.CreateAsync(command, _db.CurrentCompanyId ?? Guid.Empty, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task DeleteAsync_WhenOrderExists_SoftDeletesOrder()
    {
        // Arrange
        var order = new OrderModel { Id = Guid.NewGuid(), OrderNumber = _faker.Random.Replace("ORD-########"), SaleDate = DateTime.UtcNow, TotalAmount = _faker.Random.Decimal() };
        _db.BasicOrders.Add(order);
        await _db.SaveChangesAsync(CancellationToken.None);

        // Act
        await _service.DeleteAsync(new DeleteOrderCommand(order.Id), Guid.NewGuid(), CancellationToken.None);

        // Assert
        var deletedOrder = await _db.BasicOrders.IgnoreQueryFilters().FirstOrDefaultAsync(o => o.Id == order.Id);
        deletedOrder.Should().NotBeNull();
        deletedOrder!.Deleted.Should().NotBeNull();
    }

    [Fact]
    public async Task GetAnalyticsAsync_ReturnsAnalytics()
    {
        // Act
        var result = await _service.GetAnalyticsAsync(new GetOrderAnalyticsQuery(90, 10), CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
    }
}
