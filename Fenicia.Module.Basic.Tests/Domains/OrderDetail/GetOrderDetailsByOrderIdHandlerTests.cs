using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.OrderDetail.Handlers;
using Fenicia.Module.Basic.Domains.OrderDetail.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class GetOrderDetailsByOrderIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetOrderDetailsByOrderIdHandler handler;

    public GetOrderDetailsByOrderIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetOrderDetailsByOrderIdHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
        GC.SuppressFinalize(this);
    }

    [Fact]
    public async Task Handle_WithNoDetailsForOrder_ReturnsEmptyList()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithDetailsForOrder_ReturnsFilteredList()
    {
        // Arrange
        var order1Id = Guid.NewGuid();
        var order2Id = Guid.NewGuid();

        var detail1 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = order1Id,
            ProductId = Guid.NewGuid(),
            Price = 10.00m,
            Quantity = 5,
            DiscountAmount = 5.00m,
            Subtotal = 45.00m
        };

        var detail2 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = order1Id,
            ProductId = Guid.NewGuid(),
            Price = 20.00m,
            Quantity = 3,
            DiscountAmount = 0,
            Subtotal = 60.00m
        };

        var detail3 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = order2Id,
            ProductId = Guid.NewGuid(),
            Price = 30.00m,
            Quantity = 2,
            DiscountAmount = 0,
            Subtotal = 60.00m
        };

        db.BasicOrderDetails.AddRange(detail1, detail2, detail3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(order1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.All(result, d => Assert.Equal(order1Id, d.OrderId));
    }

    [Fact]
    public async Task Handle_VerifiesDetailDataIsCorrect()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var discountAmount = 10.00m;
        var quantity = 10;
        var subtotal = (15.00m * quantity) - discountAmount;

        var detail = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = Guid.NewGuid(),
            Price = 15.00m,
            Quantity = quantity,
            DiscountAmount = discountAmount,
            Subtotal = subtotal
        };

        db.BasicOrderDetails.Add(detail);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(15.00m, result[0].Price);
        Assert.Equal(10, result[0].Quantity);
        Assert.Equal(discountAmount, result[0].DiscountAmount);
        Assert.Equal(subtotal, result[0].Subtotal);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetOrderDetailsByOrderIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
