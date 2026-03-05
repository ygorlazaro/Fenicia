using Fenicia.Common.Data;
using Fenicia.Common.Data.Models;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Module.Basic.Domains.Order.CreateOrder;

using Microsoft.EntityFrameworkCore;
using Fenicia.Common.Data.Contexts;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

[TestFixture]
public class CreateOrderHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new CreateOrderHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private CreateOrderHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_CreatesOrderAndReturnsResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
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
            details,
            employeeId);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.UserId, Is.EqualTo(userId));
            Assert.That(result.CustomerId, Is.EqualTo(customerId));
            Assert.That(result.EmployeeId, Is.EqualTo(employeeId));
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
        var employeeId = Guid.NewGuid();
        
        // Create product with initial quantity
        var product = new BasicProductModel
        {
            Id = productId,
            Name = "Test Product",
            Quantity = 100,
            CostPrice = 5.00m,
            SalesPrice = 10.00m,
            CategoryId = Guid.NewGuid()
        };
        this.context.BasicProducts.Add(product);
        
        var details = new List<OrderDetailCommand>
        {
            new(productId, 10.00m, 5)
        };

        var command = new CreateOrderCommand(
            Guid.NewGuid(),
            customerId,
            DateTime.Now,
            OrderStatus.Pending,
            details,
            employeeId);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var stockMovements = await this.context.BasicStockMovements.ToListAsync();
        Assert.That(stockMovements, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(stockMovements[0].ProductId, Is.EqualTo(productId));
            Assert.That(stockMovements[0].CustomerId, Is.EqualTo(customerId));
            Assert.That(stockMovements[0].EmployeeId, Is.EqualTo(employeeId));
            Assert.That(stockMovements[0].Type, Is.EqualTo(StockMovementType.Out));
            Assert.That(stockMovements[0].Quantity, Is.EqualTo(5));
            Assert.That(stockMovements[0].Reason, Does.Contain("Sale order"));
        }
        
        // Verify product quantity was reduced
        var updatedProduct = await this.context.BasicProducts.FindAsync(productId);
        Assert.That(updatedProduct, Is.Not.Null);
        Assert.That(updatedProduct.Quantity, Is.EqualTo(95)); // 100 - 5
    }

    [Test]
    public async Task Handle_WithMultipleDetails_SubtractsProductQuantity()
    {
        // Arrange
        var product1 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 1",
            Quantity = 50,
            CostPrice = 5.00m,
            SalesPrice = 10.00m,
            CategoryId = Guid.NewGuid()
        };
        
        var product2 = new BasicProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            Quantity = 30,
            CostPrice = 8.00m,
            SalesPrice = 15.00m,
            CategoryId = Guid.NewGuid()
        };
        
        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var details = new List<OrderDetailCommand>
        {
            new(product1.Id, 10.00m, 5),
            new(product2.Id, 15.00m, 3)
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
        var updatedProduct1 = await this.context.BasicProducts.FindAsync(product1.Id);
        var updatedProduct2 = await this.context.BasicProducts.FindAsync(product2.Id);
        
        Assert.That(updatedProduct1.Quantity, Is.EqualTo(45)); // 50 - 5
        Assert.That(updatedProduct2.Quantity, Is.EqualTo(27)); // 30 - 3
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
        var orders = await this.context.BasicOrders.ToListAsync();
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
        var orderDetails = await this.context.BasicOrderDetails.ToListAsync();
        Assert.That(orderDetails, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithNullEmployeeId_CreatesOrderWithoutEmployee()
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
            details,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result.EmployeeId, Is.Null);
        
        var orders = await this.context.BasicOrders.ToListAsync();
        Assert.That(orders[0].EmployeeId, Is.Null);
    }
}
