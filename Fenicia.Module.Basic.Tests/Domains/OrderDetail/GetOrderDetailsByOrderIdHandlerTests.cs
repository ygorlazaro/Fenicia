using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

public class GetOrderDetailsByOrderIdHandlerTests : IDisposable
{
    public GetOrderDetailsByOrderIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, companyContext);
        this.handler = new GetOrderDetailsByOrderIdHandler(this.context);
    }

    public void Dispose()
    {
        this.context.Dispose();
        GC.SuppressFinalize(this);
    }

    private readonly DefaultContext context;
    private readonly GetOrderDetailsByOrderIdHandler handler;

    [Fact]
    public async Task Handle_WithNoDetailsForOrder_ReturnsEmptyList()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

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
            Quantity = 5
        };

        var detail2 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = order1Id,
            ProductId = Guid.NewGuid(),
            Price = 20.00m,
            Quantity = 3
        };

        var detail3 = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = order2Id,
            ProductId = Guid.NewGuid(),
            Price = 30.00m,
            Quantity = 2
        };

        this.context.BasicOrderDetails.AddRange(detail1, detail2, detail3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(order1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

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
        var detail = new OrderDetailModel
        {
            Id = Guid.NewGuid(),
            OrderId = orderId,
            ProductId = Guid.NewGuid(),
            Price = 15.00m,
            Quantity = 10
        };

        this.context.BasicOrderDetails.Add(detail);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal(15.00m, result[0].Price);
        Assert.Equal(10, result[0].Quantity);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetOrderDetailsByOrderIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }
}
