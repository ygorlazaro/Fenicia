using Bogus;

using Fenicia.Auth.Contexts;
using Fenicia.Auth.Domains.Order.Data;
using Fenicia.Auth.Domains.Order.Logic;
using Fenicia.Auth.Domains.OrderDetail.Data;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class OrderRepositoryTests
{
    private AuthContext _context;
    private OrderRepository _sut;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>()
            .UseInMemoryDatabase(databaseName: $"TestDb_{Guid.NewGuid()}")
            .Options;

        _context = new AuthContext(_options);
        _sut = new OrderRepository(_context);
        _faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Test]
    public async Task SaveOrderAsync_SavesOrder_AndReturnsOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await _sut.SaveOrderAsync(order, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(order.Id));

        var savedOrder = await _context.Orders.FindAsync([order.Id], _cancellationToken);
        Assert.That(savedOrder, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(savedOrder.CompanyId, Is.EqualTo(order.CompanyId));
            Assert.That(savedOrder.UserId, Is.EqualTo(order.UserId));
        });
    }

    [Test]
    public async Task SaveOrderAsync_PersistsOrderToDatabase()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await _sut.SaveOrderAsync(order, _cancellationToken);

        // Assert
        await using var context = new AuthContext(_options);
        var savedOrder = await context.Orders.FindAsync([order.Id], _cancellationToken);
        Assert.That(savedOrder, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(savedOrder.Id, Is.EqualTo(order.Id));
            Assert.That(savedOrder.CompanyId, Is.EqualTo(order.CompanyId));
            Assert.That(savedOrder.UserId, Is.EqualTo(order.UserId));
        });
    }

    [Test]
    public async Task SaveOrderAsync_WithOrderItems_SavesCompleteOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid(),
            Details =
            [
                new OrderDetailModel
                {
                    Id = Guid.NewGuid(),
                    ModuleId = Guid.NewGuid(),
                    Amount = _faker.Random.Int(1, 10)
                },
                new OrderDetailModel
                {
                    Id = Guid.NewGuid(),
                    ModuleId = Guid.NewGuid(),
                    Amount = _faker.Random.Int(1, 10)
                }
            ]
        };

        // Act
        var result = await _sut.SaveOrderAsync(order, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Details, Has.Count.EqualTo(2));

        var savedOrder = await _context
            .Orders.Include(o => o.Details)
            .FirstOrDefaultAsync(o => o.Id == order.Id, _cancellationToken);

        Assert.That(savedOrder, Is.Not.Null);
        Assert.That(savedOrder.Details, Has.Count.EqualTo(2));
        Assert.That(
            savedOrder.Details.Select(i => i.Id),
            Is.EquivalentTo(order.Details.Select(i => i.Id))
        );
    }
}
