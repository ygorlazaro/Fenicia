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
            SKU = "SKU001",
            Barcode = "123456789",
            Description = "Test product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            MinStockLevel = 10,
            MaxStockLevel = 500,
            ImageUrl = "http://test.com/image.jpg",
            Weight = 1.5m,
            Dimensions = "10x10x10",
            UnitOfMeasure = "un",
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(productId);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(productId, result.Id);
        Assert.Equal("Product", result.Name);
        Assert.Equal("SKU001", result.SKU);
        Assert.Equal("123456789", result.Barcode);
        Assert.Equal("Test product", result.Description);
        Assert.Equal(10, result.MinStockLevel);
        Assert.Equal(500, result.MaxStockLevel);
        Assert.True(result.IsActive);
        Assert.Equal("Electronics", result.CategoryName);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {

        var query = new GetProductByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {

        var query = new GetProductByIdQuery(Guid.NewGuid());

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithMultipleProducts_ReturnsOnlyRequestedProduct()
    {

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
            SKU = "SKU001",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };

        var product2 = new ProductModel
        {
            Id = product2Id,
            Name = "Product 2",
            SKU = "SKU002",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var query = new GetProductByIdQuery(product1Id);

        var result = await handler.Handle(query, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(product1Id, result.Id);
        Assert.Equal("Product 1", result.Name);
        Assert.NotEqual("Product 2", result.Name);
    }
}
