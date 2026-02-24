using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Inventory.GetInventoryByCategory;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

[TestFixture]
public class GetInventoryByCategoryHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetInventoryByCategoryHandler(this.context);
        this.faker = new Faker();
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetInventoryByCategoryHandler handler = null!;
    private Faker faker = null!;

    [Test]
    public async Task Handle_WithNoProductsForCategory_ReturnsEmptyInventory()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetInventoryByCategoryQuery(categoryId);

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
    public async Task Handle_WithProductsForCategory_ReturnsFilteredInventory()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel { Id = category1Id, Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = category2Id, Name = "Books" };
        this.context.ProductCategories.AddRange(category1, category2);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category1Id
        };

        var product3 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category2Id
        };

        this.context.Products.AddRange(product1, product2, product3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(category1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(2));
        Assert.That(result.Items.All(i => i.CategoryName == "Electronics"), Is.True);
    }

    [Test]
    public async Task Handle_WithProductsForCategory_CalculatesCorrectTotals()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product1 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = categoryId
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = this.faker.Commerce.ProductName(),
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = categoryId
        };

        this.context.Products.AddRange(product1, product2);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.TotalCostPrice, Is.EqualTo(25.00m));
            Assert.That(result.TotalSalesPrice, Is.EqualTo(45.00m));
            Assert.That(result.TotalQuantity, Is.EqualTo(150));
        }
    }

    [Test]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        for (var i = 0; i < 25; i++)
        {
            var product = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = this.faker.Commerce.ProductName(),
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = categoryId
            };
            this.context.Products.Add(product);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByCategoryQuery(categoryId, 2);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result.Items, Has.Count.EqualTo(10));
    }
}
