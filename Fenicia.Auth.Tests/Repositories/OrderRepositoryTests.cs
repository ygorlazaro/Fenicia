namespace Fenicia.Auth.Tests.Repositories;

using Bogus;

using Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Moq;
using Fenicia.Auth.Domains.Order;

public class OrderRepositoryTests
{
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private AuthContext _context;
    private Faker _faker;
    private DbContextOptions<AuthContext> _options;
    private OrderRepository _sut;

    [SetUp]
    public void Setup()
    {
        _options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        var mockLogger = new Mock<ILogger<OrderRepository>>().Object;
        _context = new AuthContext(_options);
        _sut = new OrderRepository(_context, mockLogger);
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
                                Amount = _faker.Random.Int(min: 1, max: 10)
                            },
                            new OrderDetailModel
                            {
                                Id = Guid.NewGuid(),
                                ModuleId = Guid.NewGuid(),
                                Amount = _faker.Random.Int(min: 1, max: 10)
                            }
                        ]
        };

        // Act
        var result = await _sut.SaveOrderAsync(order, _cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Details, Has.Count.EqualTo(expected: 2));

        var savedOrder = await _context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == order.Id, _cancellationToken);

        Assert.That(savedOrder, Is.Not.Null);
        Assert.That(savedOrder.Details, Has.Count.EqualTo(expected: 2));
        Assert.That(savedOrder.Details.Select(i => i.Id), Is.EquivalentTo(order.Details.Select(i => i.Id)));
    }
}
