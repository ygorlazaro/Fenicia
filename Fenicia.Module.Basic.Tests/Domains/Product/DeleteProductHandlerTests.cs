using Fenicia.Common.Data;
using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models;
using Fenicia.Module.Basic.Domains.Product.Delete;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class DeleteProductHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.companyContext = new TestCompanyContext();
        this.context = new DefaultContext(options, this.companyContext);
        this.handler = new DeleteProductHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private TestCompanyContext companyContext = null!;
    private DefaultContext context = null!;
    private DeleteProductHandler handler = null!;

    [Test]
    public async Task Handle_WhenProductExists_SetsDeletedDate()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new BasicProductModel
        {
            Id = productId,
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        this.context.BasicProducts.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCommand(productId);
        var beforeDelete = DateTime.Now;

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProduct = await this.context.BasicProducts.FindAsync([productId], CancellationToken.None);
        Assert.That(deletedProduct, Is.Not.Null);
        Assert.That(deletedProduct.Deleted, Is.Not.Null);
        Assert.That(deletedProduct.Deleted, Is.GreaterThanOrEqualTo(beforeDelete.AddSeconds(-1)));
        Assert.That(deletedProduct.Deleted, Is.LessThanOrEqualTo(DateTime.Now.AddSeconds(1)));
    }

    [Test]
    public async Task Handle_WhenProductDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var products = await this.context.BasicProducts.ToListAsync();
        Assert.That(products, Is.Empty);
    }

    [Test]
    public async Task Handle_WithMultipleProducts_OnlyDeletesSpecified()
    {
        // Arrange
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();

        var product1 = new BasicProductModel
        {
            Id = product1Id,
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var product2 = new BasicProductModel
        {
            Id = product2Id,
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = Guid.NewGuid()
        };

        this.context.BasicProducts.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCommand(product1Id);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProduct = await this.context.BasicProducts.FindAsync([product1Id], CancellationToken.None);
        var notDeletedProduct = await this.context.BasicProducts.FindAsync([product2Id], CancellationToken.None);

        Assert.That(deletedProduct, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(deletedProduct.Deleted, Is.Not.Null);
            Assert.That(notDeletedProduct, Is.Not.Null);
        }
        Assert.That(notDeletedProduct?.Deleted, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var products = await this.context.BasicProducts.ToListAsync();
        Assert.That(products, Is.Empty);
    }
}
