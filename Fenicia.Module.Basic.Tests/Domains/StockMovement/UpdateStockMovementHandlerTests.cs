using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement;
using Fenicia.Module.Basic.Domains.StockMovement.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

[TestFixture]
public class UpdateStockMovementHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new UpdateStockMovementHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private UpdateStockMovementHandler handler = null!;

    [Test]
    public async Task Handle_WhenStockMovementExists_UpdatesStockMovementAndReturnsResponse()
    {
        // Arrange
        var movementId = Guid.NewGuid();
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

        var movement = new StockMovementModel
        {
            Id = movementId,
            Quantity = 10,
            Date = DateTime.Now,
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };
        this.context.StockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(
            movementId,
            20,
            DateTime.Now.AddDays(1),
            25.00m,
            StockMovementType.Out,
            product.Id,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Quantity, Is.EqualTo(20));
            Assert.That(result.Type, Is.EqualTo(StockMovementType.Out));
            Assert.That(result.Price, Is.EqualTo(25.00m));
        }
    }

    [Test]
    public async Task Handle_WhenStockMovementDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.In,
            Guid.NewGuid(),
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.In,
            Guid.NewGuid(),
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesStockMovementWasUpdatedInDatabase()
    {
        // Arrange
        var movementId = Guid.NewGuid();
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

        var movement = new StockMovementModel
        {
            Id = movementId,
            Quantity = 10,
            Date = DateTime.Now,
            Price = 15.00m,
            Type = StockMovementType.In,
            ProductId = product.Id
        };
        this.context.StockMovements.Add(movement);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateStockMovementCommand(
            movementId,
            20,
            DateTime.Now.AddDays(1),
            25.00m,
            StockMovementType.Out,
            product.Id,
            null,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedMovement = await this.context.StockMovements.FindAsync([movementId], CancellationToken.None);
        Assert.That(updatedMovement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedMovement.Quantity, Is.EqualTo(20));
            Assert.That(updatedMovement.Type, Is.EqualTo(StockMovementType.Out));
        }
    }
}
