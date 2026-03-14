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
        this.db = new DefaultContext(options, companyContext);
        this.handler = new UpdateProductHandler(this.db);
    }

    public void Dispose()
    {
        this.db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenProductExists_UpdatesProductAndReturnsResponse()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category1 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        var category2 = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Books" };
        this.db.BasicProductCategories.AddRange(category1, category2);

        var product = new ProductModel
        {
            Id = productId,
            Name = "Old Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category1.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(productId, "New Product", 15.00m, 25.00m, 50, category2.Id, null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Product", result.Name);
        Assert.Equal(15.00m, result.CostPrice);
        Assert.Equal(25.00m, result.SalesPrice);
        Assert.Equal(50, result.Quantity);
        Assert.Equal(category2.Id, result.CategoryId);
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "New Product", 15.00m, 25.00m, 50, Guid.NewGuid(), null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_ReturnsNull()
    {
        // Arrange
        var command = new UpdateProductCommand(Guid.NewGuid(), "New Product", 15.00m, 25.00m, 50, Guid.NewGuid(), null);

        // Act
        var result = await this.handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public async Task Handle_VerifiesProductWasUpdatedInDatabase()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var category = new ProductCategoryModel { Id = Guid.NewGuid(), Name = "Electronics" };
        this.db.BasicProductCategories.Add(category);

        var product = new ProductModel
        {
            Id = productId,
            Name = "Old Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = category.Id
        };

        this.db.BasicProducts.Add(product);
        await this.db.SaveChangesAsync(CancellationToken.None);

        var command = new UpdateProductCommand(productId, "New Product", 15.00m, 25.00m, 50, category.Id, null);

        // Act
        await this.handler.Handle(command, CancellationToken.None);

        // Assert
        var updatedProduct = await this.db.BasicProducts.FindAsync([productId], CancellationToken.None);
        Assert.NotNull(updatedProduct);
        Assert.Equal("New Product", updatedProduct.Name);
        Assert.Equal(15.00m, updatedProduct.CostPrice);
    }
}