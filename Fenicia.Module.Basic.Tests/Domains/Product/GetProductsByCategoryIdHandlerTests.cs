using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductsByCategoryIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetProductsByCategoryIdHandler handler;

    public GetProductsByCategoryIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProductsByCategoryIdHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithNoProductsForCategory_ReturnsEmptyList()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var query = new GetProductsByCategoryIdQuery(categoryId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_WithProductsForCategory_ReturnsFilteredList()
    {
        // Arrange
        var category1Id = Guid.NewGuid();
        var category2Id = Guid.NewGuid();

        var category1 = new ProductCategoryModel
        {
            Id = category1Id,
            Name = "Electronics"
        };
        var category2 = new ProductCategoryModel
        {
            Id = category2Id,
            Name = "Books"
        };
        db.BasicProductCategories.AddRange(category1, category2);

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

        db.BasicProducts.AddRange(product1, product2, product3);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(category1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.True(result.All(p => p.CategoryId == category1Id));
    }

    [Fact]
    public async Task Handle_WithPagination_ReturnsCorrectPage()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

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
            db.BasicProducts.Add(product);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId, 2);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(10, result.Count);
    }

    [Fact]
    public async Task Handle_WithPageBeyondData_ReturnsEmptyList()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

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
            db.BasicProducts.Add(product);
        }

        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId, 10);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryDataIsIncluded()
    {
        // Arrange
        var categoryId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = categoryId,
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = Guid.NewGuid(),
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = categoryId
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductsByCategoryIdQuery(categoryId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Electronics", result[0].CategoryName);
    }
}
