using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.OrderDetail;
using Fenicia.Module.Basic.Domains.OrderDetail.GetByOrderId;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.OrderDetail;

[TestFixture]
public class GetOrderDetailsByOrderIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetOrderDetailsByOrderIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetOrderDetailsByOrderIdHandler handler = null!;

    [Test]
    public async Task Handle_WithNoDetailsForOrder_ReturnsEmptyList()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
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

        this.context.OrderDetails.AddRange(detail1, detail2, detail3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(order1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(d => d.OrderId == order1Id), Is.True);
    }

    [Test]
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

        this.context.OrderDetails.Add(detail);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetOrderDetailsByOrderIdQuery(orderId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result[0].Price, Is.EqualTo(15.00m));
            Assert.That(result[0].Quantity, Is.EqualTo(10));
        }
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsEmptyList()
    {
        // Arrange
        var query = new GetOrderDetailsByOrderIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
