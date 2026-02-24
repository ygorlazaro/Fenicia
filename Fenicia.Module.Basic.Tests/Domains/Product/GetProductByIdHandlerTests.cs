using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.GetById;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class GetProductByIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetProductByIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetProductByIdHandler handler = null!;

    [Test]
    public async Task Handle_WhenProductExists_ReturnsProductResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(productId));
            Assert.That(result.Name, Is.EqualTo("Product"));
            Assert.That(result.CategoryName, Is.EqualTo("Electronics"));
        }
    }

    [Test]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task Handle_WithMultipleProducts_ReturnsOnlyRequestedProduct()
    {
        // Arrange
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = product1Id,
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        var product2 = new ProductModel
        {
            Id = product2Id,
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id
        };

        this.context.Products.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(product1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Id, Is.EqualTo(product1Id));
            Assert.That(result.Name, Is.EqualTo("Product 1"));
        }
        Assert.That(result.Name, Is.Not.EqualTo("Product 2"));
    }
}
