using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

[TestFixture]
public class CreateOrderHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new CreateOrderHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private CreateOrderHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_CreatesOrderAndReturnsResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var details = new List<OrderDetailCommand>
        {
            new(Guid.NewGuid(), 10.00m, 5),
            new(Guid.NewGuid(), 20.00m, 3)
        };

        var command = new CreateOrderCommand(
            userId,
            customerId,
            DateTime.Now,
            OrderStatus.Pending,
            details);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.CustomerId, Is.EqualTo(customerId));
            Assert.That(result.TotalAmount, Is.EqualTo(110.00m));
            Assert.That(result.SaleDate, Is.EqualTo(command.SaleDate));
            Assert.That(result.Status, Is.EqualTo(OrderStatus.Pending));
        }
    }

    [Test]
    public async Task Handle_WithSingleDetail_CalculatesCorrectTotalAmount()
    {
        // Arrange
        var details = new List<OrderDetailCommand>
        {
            new(Guid.NewGuid(), 15.00m, 2)
        };

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            OrderStatus.Pending,
            details);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.TotalAmount, Is.EqualTo(30.00m));
    }

    [Test]
    public async Task Handle_WithMultipleDetails_CreatesStockMovements()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var details = new List<OrderDetailCommand>
        {
            new(productId, 10.00m, 5)
        };

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            customerId,
            DateTime.Now,
            OrderStatus.Pending,
            details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var stockMovements = await this.context.StockMovements.ToListAsync();
        Assert.That(stockMovements, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(stockMovements[0].ProductId, Is.EqualTo(productId));
            Assert.That(stockMovements[0].CustomerId, Is.EqualTo(customerId));
            Assert.That(stockMovements[0].Type, Is.EqualTo(StockMovementType.In));
            Assert.That(stockMovements[0].Quantity, Is.EqualTo(5));
        }
    }

    [Test]
    public async Task Handle_VerifiesOrderWasSavedToDatabase()
    {
        // Arrange
        var details = new List<OrderDetailCommand>
        {
            new(Guid.NewGuid(), 10.00m, 5)
        };

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            OrderStatus.Pending,
            details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var orders = await this.context.Orders.ToListAsync();
        Assert.That(orders, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(orders[0].CustomerId, Is.EqualTo(command.CustomerId));
            Assert.That(orders[0].Status, Is.EqualTo(OrderStatus.Pending));
        }
    }

    [Test]
    public async Task Handle_WithMultipleDetails_CreatesOrderDetails()
    {
        // Arrange
        var details = new List<OrderDetailCommand>
        {
            new(Guid.NewGuid(), 10.00m, 5),
            new(Guid.NewGuid(), 20.00m, 3)
        };

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            Guid.NewGuid(),
            DateTime.Now,
            OrderStatus.Pending,
            details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var orderDetails = await this.context.OrderDetails.ToListAsync();
        Assert.That(orderDetails, Has.Count.EqualTo(2));
    }
}
