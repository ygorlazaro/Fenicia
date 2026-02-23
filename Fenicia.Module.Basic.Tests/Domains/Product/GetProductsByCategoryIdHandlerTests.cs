using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Module.Basic.Domains.Product.GetAll;
using Fenicia.Module.Basic.Domains.Product.GetByCategoryId;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

[TestFixture]
public class GetProductsByCategoryIdHandlerTests
{
    [SetUp]
    public void SetUp()
    {
        var options = new DbContextOptionsBuilder<BasicContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        this.context = new BasicContext(options);
        this.handler = new GetProductsByCategoryIdHandler(this.context);
    }

    [TearDown]
    public void TearDown()
    {
        this.context.Dispose();
    }

    private BasicContext context = null!;
    private GetProductsByCategoryIdHandler handler = null!;

    [Test]
    public async Task Handle_WithNoProductsForCategory_ReturnsEmptyList()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetProductsByCategoryIdQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_WithProductsForCategory_ReturnsFilteredList()
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
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1Id
        };

        var product2 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category1Id
        };

        var product3 = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product 3",
            CostPrice = 20.00m,
            SalesPrice = 30.00m,
            Quantity = 75,
            CategoryId = category2Id
        };

        this.context.Products.AddRange(product1, product2, product3);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(category1Id);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(2));
        Assert.That(result.All(p => p.CategoryId == category1Id), Is.True);
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
                Name = $"Product {i}",
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = categoryId
            };
            this.context.Products.Add(product);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId, 2);

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
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        for (var i = 0; i < 5; i++)
        {
            var product = new ProductModel
            {
                Id = Guid.NewGuid(),
                Name = $"Product {i}",
                CostPrice = 10.00m,
                SalesPrice = 20.00m,
                Quantity = 100,
                CategoryId = categoryId
            };
            this.context.Products.Add(product);
        }

        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId, 10);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Is.Empty);
    }

    [Test]
    public async Task Handle_VerifiesCategoryDataIsIncluded()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = categoryId, Name = "Electronics" };
        this.context.ProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = categoryId
        };

        this.context.Products.Add(product);
        await this.context.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId);

        // Act
        var result = await this.handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.That(result, Is.Not.Null);
        Assert.That(result, Has.Count.EqualTo(1));
        Assert.That(result[0].CategoryName, Is.EqualTo("Electronics"));
    }
}
