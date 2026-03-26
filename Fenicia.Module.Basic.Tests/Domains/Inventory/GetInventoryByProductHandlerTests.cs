using Bogus;

using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Inventory.Handlers;
using Fenicia.Module.Basic.Domains.Inventory.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Inventory;

/// <summary>
///     Unit tests for the GetInventoryByProductHandler.
///     Tests inventory retrieval by product ID.
/// </summary>
public class GetInventoryByProductHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly Faker faker;
    private readonly GetInventoryByProductHandler handler;

    public GetInventoryByProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetInventoryByProductHandler(db);
        faker = new Faker();
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WithNonExistentProduct_ReturnsEmptyInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCostPrice);
        Assert.Equal(0, result.TotalSalesPrice);
        Assert.Equal(0, result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithExistingProduct_ReturnsProductInventory()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(productId, result.Items[0].Id);
        Assert.Equal(product.Name, result.Items[0].Name);
        Assert.Equal(10.00m, result.TotalCostPrice);
        Assert.Equal(20.00m, result.TotalSalesPrice);
        Assert.Equal(100, result.TotalQuantity);
    }

    [Fact]
    public async Task Handle_WithProductHavingNullCostPrice_HandlesCorrectly()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = faker.Commerce.ProductName(),
            CostPrice = null,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal(0, result.TotalCostPrice);
        Assert.Equal(20.00m, result.TotalSalesPrice);
    }

    [Fact]
    public async Task Handle_VerifiesCategoryNameIsIncluded()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = faker.Commerce.ProductName(),
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetInventoryByProductQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        Assert.Equal("Electronics", result.Items[0].CategoryName);
    }
}
