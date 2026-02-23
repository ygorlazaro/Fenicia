using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByProduct;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

[TestFixture]
public class GetInventoryByProductHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetInventoryByProductHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetInventoryByProductHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithNonExistentProduct_ReturnsEmptyInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items, Is.Empty);
            Assert.That(result.TotalCostPrice, Is.EqualTo(0));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(0));
            Assert.That(result.TotalQuantity, Is.EqualTo(0));
        }
    }

    [Test]
    public async Task Handle_WithExistingProduct_ReturnsProductInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(1));
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items[0].Id, Is.EqualTo(productId));
            Assert.That(result.Items[0].Name, Is.EqualTo(product.Name));
            Assert.That(result.TotalCostPrice, Is.EqualTo(10.00m));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(20.00m));
            Assert.That(result.TotalQuantity, Is.EqualTo(100));
        }
    }

    [Test]
    public async Task Handle_WithProductHavingNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = null,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.Items, Has.Count.EqualTo(1));
            Assert.That(result.TotalCostPrice, Is.EqualTo(0));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(20.00m));
        }
    }

    [Test]
    public async Task Handle_VerifiesCategoryNameIsIncluded()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(1));
        Assert.That(result.Items[0].CategoryName, Is.EqualTo("Electronics"));
    }
}
