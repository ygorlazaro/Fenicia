using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Enums.Basic;
using Fenicia.Module.Basic.Domains.StockMovement.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.StockMovement;

[TestFixture]
public class AddStockMovementHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddStockMovementHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddStockMovementHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsStockMovementAndReturnsResponse()
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
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.In,
            product.Id,
            null,
            null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.ProductId, Is.EqualTo(command.ProductId));
            Assert.That(result.Quantity, Is.EqualTo(command.Quantity));
            Assert.That(result.Type, Is.EqualTo(command.Type));
        }
    }

    [Test]
    public async Task Handle_WithStockMovementIn_IncreasesProductQuantity()
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
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.In,
            product.Id,
            null,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.context.Products.FindAsync([product.Id], CancellationToken.None);
        Assert.That(updatedProduct, Is.Not.Null);
        Assert.That(updatedProduct.Quantity, Is.EqualTo(110));
    }

    [Test]
    public async Task Handle_WithStockMovementOut_DecreasesProductQuantity()
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
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.Out,
            product.Id,
            null,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.context.Products.FindAsync([product.Id], CancellationToken.None);
        Assert.That(updatedProduct, Is.Not.Null);
        Assert.That(updatedProduct.Quantity, Is.EqualTo(90));
    }

    [Test]
    public async Task Handle_WithNullProduct_DoesNotUpdateQuantity()
    {
        // Arrange
        var command = new AddStockMovementCommand(
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
        Assert.That(result, Is.Not.Null);
        var movement = await this.context.StockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.That(movement, Is.Not.Null);
    }

    [Test]
    public async Task Handle_VerifiesStockMovementWasSavedToDatabase()
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
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new AddStockMovementCommand(
            Guid.NewGuid(),
            10,
            DateTime.Now,
            15.00m,
            StockMovementType.In,
            product.Id,
            null,
            null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var movement = await this.context.StockMovements.FindAsync([command.Id], CancellationToken.None);
        Assert.That(movement, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(movement.Quantity, Is.EqualTo(command.Quantity));
            Assert.That(movement.Type, Is.EqualTo(command.Type));
        }
    }
}
