using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class UpdateProductHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly UpdateProductHandler handler;

    public UpdateProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new UpdateProductHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenProductExists_UpdatesProductAndReturnsResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category1 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Electronics"
        };
        var category2 = new ProductCategoryModel
        {
            Id = Guid.NewGuid(),
            Name = "Books"
        };
        db.BasicProductCategories.AddRange(category1, category2);

        var product = new ProductModel
        {
            Id = productId,
            Name = "Old Product",
            SKU = "OLD001",
            Barcode = "111111111",
            Description = "Old description",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1.Id,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(productId, "New Product", "NEW001", "999999999", "New description", 15.00m, 25.00m, 50, 5, 200, "http://new.com", 2.5m, "20x20x20", "kg", category2.Id, null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal("NEW001", result.SKU);
        Assert.Equal("999999999", result.Barcode);
        Assert.Equal("New description", result.Description);
        Assert.Equal(15.00m, result.CostPrice);
        Assert.Equal(25.00m, result.SalesPrice);
        Assert.Equal(50, result.Quantity);
        Assert.Equal(5, result.MinStockLevel);
        Assert.Equal(200, result.MaxStockLevel);
        Assert.Equal(category2.Id, result.CategoryId);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "New Product", "SKU001", "123456789", "Desc", 15.00m, 25.00m, 50, 5, 200, "http://img.com", 1.5m, "10x10x10", "un", Guid.NewGuid(), null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "New Product", "SKU001", "123456789", "Desc", 15.00m, 25.00m, 50, 5, 200, "http://img.com", 1.5m, "10x10x10", "un", Guid.NewGuid(), null);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesProductWasUpdatedInDatabase()
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
            Name = "Old Product",
            SKU = "OLD001",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id,
            IsActive = true
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(productId, "New Product", "NEW001", "999999999", "New desc", 15.00m, 25.00m, 50, 5, 200, "http://img.com", 2.0m, "20x20x20", "kg", category.Id, null);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await db.BasicProducts.FindAsync([productId], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal("New Product", updatedProduct.Name);
        Assert.Equal("NEW001", updatedProduct.SKU);
        Assert.Equal(15.00m, updatedProduct.CostPrice);
    }
}
