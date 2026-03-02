using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Product.Update;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class UpdateProductHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new UpdateProductHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private UpdateProductHandler handler = null!;

    [Test]
    public async Task Handle_WhenProductExists_UpdatesProductAndReturnsResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category1 = new BasicProductCategory { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new BasicProductCategory { Id = Guid.NewGuid(), Name = "Books" };
        this.context.BasicProductCategories.AddRange(category1, category2);

        var product = new BasicProduct
        {
            Id = productId,
            Name = "Old Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1.Id
        };

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(
            productId,
            "New Product",
            15.00m,
            25.00m,
            50,
            category2.Id);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Name, Is.EqualTo("New Product"));
            Assert.That(result.CostPrice, Is.EqualTo(15.00m));
            Assert.That(result.SalesPrice, Is.EqualTo(25.00m));
            Assert.That(result.Quantity, Is.EqualTo(50));
            Assert.That(result.CategoryId, Is.EqualTo(category2.Id));
        }
    }

    [Test]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "New Product",
            15.00m,
            25.00m,
            50,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(
            Guid.NewGuid(),
            "New Product",
            15.00m,
            25.00m,
            50,
            Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_VerifiesProductWasUpdatedInDatabase()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new BasicProductCategory { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.BasicProductCategories.Add(category);

        var product = new BasicProduct
        {
            Id = productId,
            Name = "Old Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(
            productId,
            "New Product",
            15.00m,
            25.00m,
            50,
            category.Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.context.BasicProducts.FindAsync([productId], CancellationToken.None);
        Assert.That(updatedProduct, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(updatedProduct.Name, Is.EqualTo("New Product"));
            Assert.That(updatedProduct.CostPrice, Is.EqualTo(15.00m));
        }
    }
}
