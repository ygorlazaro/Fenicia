using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Auth;
using Fenicia.Common.Enums.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Order.Commands;
using Fenicia.Module.Basic.Domains.Order.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Order;

public class CreateOrderHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly CreateOrderHandler handler;

    public CreateOrderHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        this.db = new DefaultContext(options, companyContext);
        this.handler = new CreateOrderHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesOrderAndReturnsResponse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5), new(Guid.NewGuid(), 20.00m, 3) };

        var command = new CreateOrderCommand(userId, customerId, DateTime.Now, OrderStatus.Pending, details, employeeId);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal(110.00m, result.TotalAmount);
        Assert.Equal(command.SaleDate, result.SaleDate);
        Assert.Equal(OrderStatus.Pending, result.Status);
    }

    [Fact]
    public async Task Handle_WithSingleDetail_CalculatesCorrectTotalAmount()
    {
        // Arrange
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 15.00m, 2) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(30.00m, result.TotalAmount);
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_CreatesStockMovements()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        // Create product with initial quantity
        var product = new ProductModel
        {
            Id = productId,
            Name = "Test Product",
            Quantity = 100,
            CostPrice = 5.00m,
            SalesPrice = 10.00m,
            CategoryId = Guid.NewGuid()
        };
        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var details = new List<OrderDetailCommand> { new(productId, 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), customerId, DateTime.Now, OrderStatus.Pending, details, employeeId);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var stockMovements = await this.db.BasicStockMovements.ToListAsync();
        Assert.Single(stockMovements);
        Assert.Equal(productId, stockMovements[0].ProductId);
        Assert.Equal(customerId, stockMovements[0].CustomerId);
        Assert.Equal(employeeId, stockMovements[0].EmployeeId);
        Assert.Equal(StockMovementType.Out, stockMovements[0].Type);
        Assert.Equal(5, stockMovements[0].Quantity);
        Assert.Contains("Sale order", stockMovements[0].Reason);

        // Verify product quantity was reduced
        var updatedProduct = await this.db.BasicProducts.FindAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Equal(95, updatedProduct.Quantity); // 100 - 5
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_SubtractsProductQuantity()
    {
        // Arrange
        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 1",
            Quantity = 50,
            CostPrice = 5.00m,
            SalesPrice = 10.00m,
            CategoryId = Guid.NewGuid()
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            Quantity = 30,
            CostPrice = 8.00m,
            SalesPrice = 15.00m,
            CategoryId = Guid.NewGuid()
        };

        this.db.BasicProducts.AddRange(product1, product2);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var details = new List<OrderDetailCommand> { new(product1.Id, 10.00m, 5), new(product2.Id, 15.00m, 3) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct1 = await this.db.BasicProducts.FindAsync(product1.Id);
        var updatedProduct2 = await this.db.BasicProducts.FindAsync(product2.Id);
        Assert.Equal(45, updatedProduct1?.Quantity); // 50 - 5
        Assert.Equal(27, updatedProduct2?.Quantity); // 30 - 3
    }

    [Fact]
    public async Task Handle_VerifiesOrderWasSavedToDatabase()
    {
        // Arrange
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var orders = await this.db.BasicOrders.ToListAsync();
        Assert.Single(orders);
        Assert.Equal(command.CustomerId, orders[0].CustomerId);
        Assert.Equal(OrderStatus.Pending, orders[0].Status);
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_CreatesOrderDetails()
    {
        // Arrange
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5), new(Guid.NewGuid(), 20.00m, 3) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var orderDetails = await this.db.BasicOrderDetails.ToListAsync();
        Assert.Equal(2, orderDetails.Count);
    }

    [Fact]
    public async Task Handle_WithNullEmployeeId_CreatesOrderWithoutEmployee()
    {
        // Arrange
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result.EmployeeId);

        var orders = await this.db.BasicOrders.ToListAsync();
        Assert.Null(orders[0].EmployeeId);
    }
}