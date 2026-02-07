using Bogus;

using Fenicia.Auth.Domains.Order;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Auth;

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
        this.options = new DbContextOptionsBuilder<AuthContext>().UseInMemoryDatabase($"TestDb_{Guid.NewGuid()}").Options;

        this.context = new AuthContext(this.options);
        this.sut = new OrderRepository(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Database.EnsureDeleted();
        this.context.Dispose();
    }

    [Test]
    public async Task SaveOrderAsyncSavesOrderAndReturnsOrder()
    {
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        this.sut.Add(order);

        await this.sut.SaveChangesAsync(this.cancellationToken);

        var savedOrder = await this.context.Orders.FindAsync([order.Id], this.cancellationToken);
        Assert.That(savedOrder, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(savedOrder.Id, Is.EqualTo(order.Id));
            Assert.That(savedOrder.CompanyId, Is.EqualTo(order.CompanyId));
            Assert.That(savedOrder.UserId, Is.EqualTo(order.UserId));
        }
    }

    [Test]
    public async Task SaveOrderAsyncPersistsOrderToDatabase()
    {
        var order = new OrderModel
        {
            Id = Guid.NewGuid(),
            CompanyId = Guid.NewGuid(),
            UserId = Guid.NewGuid()
        };

        this.sut.Add(order);
        await this.sut.SaveChangesAsync(this.cancellationToken);

        await using var authContext = new AuthContext(this.options);
        var savedOrder = await authContext.Orders.FindAsync([order.Id], this.cancellationToken);
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
                    Price = this.faker.Random.Int(1, 10)
                },
                new OrderDetailModel
                {
                    Id = Guid.NewGuid(),
                    ModuleId = Guid.NewGuid(),
                    Price = this.faker.Random.Int(1, 10)
                }

            ]
        };

        this.sut.Add(order);

        await this.sut.SaveChangesAsync(this.cancellationToken);
        var savedOrder = await this.context.Orders.Include(o => o.Details).FirstOrDefaultAsync(o => o.Id == order.Id, this.cancellationToken);

        Assert.That(savedOrder, Is.Not.Null);
        Assert.That(savedOrder.Details, Has.Count.EqualTo(2));
        Assert.That(savedOrder.Details.Select(i => i.Id), Is.EquivalentTo(order.Details.Select(i => i.Id)));
    }
}