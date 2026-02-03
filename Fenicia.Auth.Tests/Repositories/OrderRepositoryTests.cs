using Bogus;

using Fenicia.Auth.Domains.Order;
using Fenicia.Common.Database.Contexts;
using Fenicia.Common.Database.Models.Auth;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Auth.Tests.Repositories;

public class OrderRepositoryTests
{
    private readonly CancellationToken cancellationToken = CancellationToken.None;
    private AuthContext context;
    private Faker faker;
    private DbContextOptions<AuthContext> options;
    private OrderRepository sut;

    [SetUp]
    public void Setup()
    {
        options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        context = new AuthContext(options);
        sut = new OrderRepository(context);
        faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    [Test]
    public async Task SaveOrderAsyncSavesOrderAndReturnsOrder()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        var result = await sut.SaveOrderAsync(order, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Id, Is.EqualTo(order.Id));

        var savedOrder = await context.Orders.FindAsync([order.Id], cancellationToken);
        Assert.That(savedOrder, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedOrder.CompanyId, Is.EqualTo(order.CompanyId));
            Assert.That(savedOrder.UserId, Is.EqualTo(order.UserId));
        }
    }

    [Test]
    public async Task SaveOrderAsyncPersistsOrderToDatabase()
    {
        // Arrange
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        // Act
        await sut.SaveOrderAsync(order, cancellationToken);

        // Assert
        await using var authContext = new AuthContext(options);
        var savedOrder = await authContext.Orders.FindAsync([order.Id], cancellationToken);
        Assert.That(savedOrder, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedOrder.Id, Is.EqualTo(order.Id));
            Assert.That(savedOrder.CompanyId, Is.EqualTo(order.CompanyId));
            Assert.That(savedOrder.UserId, Is.EqualTo(order.UserId));
        }
    }

    [Test]
    public async Task SaveOrderAsyncWithOrderItemsSavesCompleteOrder()
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
                                Amount = faker.Random.Int(min: 1, max: 10)
                            },
                            new OrderDetailModel
                            {
                                Id = Guid.NewGuid(),
                                ModuleId = Guid.NewGuid(),
                                Amount = faker.Random.Int(min: 1, max: 10)
                            }

                        ]
        };

        // Act
        var result = await sut.SaveOrderAsync(order, cancellationToken);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Details, Has.Count.EqualTo(expected: 2));

        var savedOrder = await context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == order.Id, cancellationToken);

        Assert.That(savedOrder, Is.Not.Null);
        Assert.That(savedOrder.Details, Has.Count.EqualTo(expected: 2));
        Assert.That(savedOrder.Details.Select(i => i.Id), Is.EquivalentTo(order.Details.Select(i => i.Id)));
    }
}
