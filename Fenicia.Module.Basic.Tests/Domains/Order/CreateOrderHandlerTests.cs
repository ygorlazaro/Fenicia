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
        db = new DefaultContext(options, companyContext);
        handler = new CreateOrderHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithValidCommand_CreatesOrderAndReturnsResponse()
    {

        var userId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();
        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5), new(Guid.NewGuid(), 20.00m, 3) };

        var command = new CreateOrderCommand(userId, customerId, DateTime.Now, OrderStatus.Pending, details, PaymentMethod.CreditCard, employeeId);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(userId, result.UserId);
        Assert.Equal(customerId, result.CustomerId);
        Assert.Equal(employeeId, result.EmployeeId);
        Assert.Equal(110.00m, result.TotalAmount);
        Assert.Equal(command.SaleDate, result.SaleDate);
        Assert.Equal(OrderStatus.Pending, result.Status);
        Assert.NotNull(result.OrderNumber);
        Assert.StartsWith("ORD-", result.OrderNumber);
        Assert.Equal(PaymentMethod.CreditCard, result.PaymentMethod);
    }

    [Fact]
    public async Task Handle_WithSingleDetail_CalculatesCorrectTotalAmount()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 15.00m, 2) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.Pix);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(30.00m, result.TotalAmount);
        Assert.Equal(2, result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_CreatesStockMovements()
    {

        var productId = Guid.NewGuid();
        var customerId = Guid.NewGuid();
        var employeeId = Guid.NewGuid();

        var product = new ProductModel
        {
            Id = productId,
            Name = "Test Product",
            Quantity = 100,
            CostPrice = 5.00m,
            SalesPrice = 10.00m,
            CategoryId = Guid.NewGuid()
        };
        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var details = new List<OrderDetailCommand> { new(productId, 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), customerId, DateTime.Now, OrderStatus.Pending, details, PaymentMethod.CreditCard, employeeId);

        await handler.Handle(command, CancellationToken.None);

        var stockMovements = await db.BasicStockMovements.ToListAsync();
        Assert.Single(stockMovements);
        Assert.Equal(productId, stockMovements[0].ProductId);
        Assert.Equal(customerId, stockMovements[0].CustomerId);
        Assert.Equal(employeeId, stockMovements[0].EmployeeId);
        Assert.Equal(StockMovementType.Out, stockMovements[0].Type);
        Assert.Equal(5, stockMovements[0].Quantity);
        Assert.Contains("Sale order", stockMovements[0].Reason);

        var updatedProduct = await db.BasicProducts.FindAsync(productId);
        Assert.NotNull(updatedProduct);
        Assert.Equal(95, updatedProduct.Quantity);
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_SubtractsProductQuantity()
    {

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

        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var details = new List<OrderDetailCommand> { new(product1.Id, 10.00m, 5), new(product2.Id, 15.00m, 3) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.Cash);

        await handler.Handle(command, CancellationToken.None);

        var updatedProduct1 = await db.BasicProducts.FindAsync(product1.Id);
        var updatedProduct2 = await db.BasicProducts.FindAsync(product2.Id);
        Assert.Equal(45, updatedProduct1?.Quantity);
        Assert.Equal(27, updatedProduct2?.Quantity);
    }

    [Fact]
    public async Task Handle_VerifiesOrderWasSavedToDatabase()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.Boleto);

        await handler.Handle(command, CancellationToken.None);

        var orders = await db.BasicOrders.ToListAsync();
        Assert.Single(orders);
        Assert.Equal(command.CustomerId, orders[0].CustomerId);
        Assert.Equal(OrderStatus.Pending, orders[0].Status);
        Assert.Equal(PaymentMethod.Boleto, orders[0].PaymentMethod);
    }

    [Fact]
    public async Task Handle_WithMultipleDetails_CreatesOrderDetails()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5), new(Guid.NewGuid(), 20.00m, 3) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.DebitCard);

        await handler.Handle(command, CancellationToken.None);

        var orderDetails = await db.BasicOrderDetails.ToListAsync();
        Assert.Equal(2, orderDetails.Count);
    }

    [Fact]
    public async Task Handle_WithNullEmployeeId_CreatesOrderWithoutEmployee()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.Pix);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.Null(result.EmployeeId);

        var orders = await db.BasicOrders.ToListAsync();
        Assert.Null(orders[0].EmployeeId);
    }

    [Fact]
    public async Task Handle_WithDiscount_CalculatesCorrectSubtotal()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 50.00m, 2, 10.00m) };

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.CreditCard);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(90.00m, result.TotalAmount);
    }

    [Fact]
    public async Task Handle_WithNotes_StoresNotesCorrectly()
    {

        var details = new List<OrderDetailCommand> { new(Guid.NewGuid(), 10.00m, 5) };
        var notes = "Please deliver in the morning";

        var command = new CreateOrderCommand(Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, OrderStatus.Pending, details, PaymentMethod.CreditCard, null, notes);

        var result = await handler.Handle(command, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(notes, result.Notes);
    }
}
