using Fenicia.Common.Data.Contexts;
using Fenicia.Module.Basic.Domains.Product.Add;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class AddProductHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new AddProductHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private AddProductHandler handler = null!;

    [Test]
    public async Task Handle_WithValidCommand_AddsProductAndReturnsResponse()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var command = new AddProductCommand(
            Guid.NewGuid(),
            "Product Name",
            10.00m,
            20.00m,
            100,
            categoryId);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(command.Id));
            Assert.That(result.Name, Is.EqualTo(command.Name));
            Assert.That(result.CostPrice, Is.EqualTo(command.CostPrice));
            Assert.That(result.SalesPrice, Is.EqualTo(command.SellingPrice));
            Assert.That(result.Quantity, Is.EqualTo(command.Quantity));
            Assert.That(result.CategoryId, Is.EqualTo(command.CategoryId));
        }
    }

    [Test]
    public async Task Handle_VerifiesProductWasSavedToDatabase()
    {
        // Arrange
        var command = new AddProductCommand(
            Guid.NewGuid(),
            "Product Name",
            10.00m,
            20.00m,
            100,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var product = await this.context.Products.FindAsync([command.Id], CancellationToken.None);
        Assert.That(product, Is.Not.Null);
        Assert.That(product.Name, Is.EqualTo(command.Name));
    }

    [Test]
    public async Task Handle_WithMultipleCommands_AddsAllProducts()
    {
        // Arrange
        var command1 = new AddProductCommand(
            Guid.NewGuid(),
            "Product 1",
            10.00m,
            20.00m,
            100,
            Guid.NewGuid());

        var command2 = new AddProductCommand(
            Guid.NewGuid(),
            "Product 2",
            15.00m,
            25.00m,
            50,
            Guid.NewGuid());

        // Act
        await this.handler.Handle(command1, CancellationToken.None);
        await this.handler.Handle(command2, CancellationToken.None);

        // Assert
        var products = await this.context.Products.ToListAsync();
        Assert.That(products, Has.Count.EqualTo(2));
    }

    [Test]
    public async Task Handle_WithNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var command = new AddProductCommand(
            Guid.NewGuid(),
            "Product Name",
            null,
            20.00m,
            100,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.CostPrice, Is.Null);
    }
}
