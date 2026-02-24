using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.GetMovement;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

[TestFixture]
public class GetStockMovementHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetStockMovementHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetStockMovementHandler handler = null!;

    [Test]
    public async Task Handle_WithNoMovementsInDateRange_ReturnsEmptyList()
    {
        // Arrange
        var startDate = DateTime.Now.AddDays(-10);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMovementsInDateRange_ReturnsFilteredList()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.context.Products.Add(product);

        var movement1 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 10,
            Date = DateTime.Now.AddDays(-5),
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };

        var movement2 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 20,
            Date = DateTime.Now.AddDays(-2),
            Price = 25.00m,
            Type = StockMovementType.Out,
            ProductId = product.Id
        };

        var movement3 = new StockMovementModel
        {
            Id = Guid.NewGuid(),
            Quantity = 30,
            Date = DateTime.Now.AddDays(5),
            Price = 35.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };

        this.context.StockMovements.AddRange(movement1, movement2, movement3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-10);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(m => m.Date >= startDate && m.Date <= endDate), Is.True);
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.context.Products.Add(product);

        for (var i = 0; i < 25; i++)
        {
            var movement = new StockMovementModel
            {
                Id = Guid.NewGuid(),
                Quantity = 10,
                Date = DateTime.Now.AddDays(-i),
                Price = 15.00m,
                Type = StockMovementType.In,
                ProductId = product.Id
            };
            this.context.StockMovements.Add(movement);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate, 2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(10));
    }

    [Test]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };
        this.context.Products.Add(product);

        for (var i = 0; i < 5; i++)
        {
            var movement = new StockMovementModel
            {
                Id = Guid.NewGuid(),
                Quantity = 10,
                Date = DateTime.Now.AddDays(-i),
                Price = 15.00m,
                Type = StockMovementType.In,
                ProductId = product.Id
            };
            this.context.StockMovements.Add(movement);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var startDate = DateTime.Now.AddDays(-30);
        var endDate = DateTime.Now;
        var query = new GetStockMovementQuery(startDate, endDate, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }
}
