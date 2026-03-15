using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Handlers;
using Fenicia.Module.Basic.Domains.Product.Queries;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class GetProductByIdHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly GetProductByIdHandler handler;

    public GetProductByIdHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new GetProductByIdHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenProductExists_ReturnsProductResponse()
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
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(productId);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Product", result.Name);
        Assert.Equal("Electronics", result.CategoryName);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var query = new GetProductByIdQuery(Guid.NewGuid());

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProducts_ReturnsOnlyRequestedProduct()
    {
        // Arrange
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();
        var category = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        db.BasicProductCategories.Add(category);

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

        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(product1Id);

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(product1Id, result.Id);
        Assert.Equal("Product 1", result.Name);
        Assert.NotEqual("Product 2", result.Name);
    }
}
