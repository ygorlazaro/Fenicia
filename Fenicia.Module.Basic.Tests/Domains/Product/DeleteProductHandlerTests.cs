using Fenicia.Common.Data.Contexts;
using Fenicia.Common.Data.Models.Basic;
using Fenicia.Common.Tests;
using Fenicia.Module.Basic.Domains.Product.Commands;
using Fenicia.Module.Basic.Domains.Product.Handlers;

using Microsoft.EntityFrameworkCore;

namespace Fenicia.Module.Basic.Tests.Domains.Product;

public class DeleteProductHandlerTests : IDisposable
{
    private readonly DefaultContext db;
    private readonly DeleteProductHandler handler;

    public DeleteProductHandlerTests()
    {
        var options = new DbContextOptionsBuilder<DefaultContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;

        var companyContext = new TestCompanyContext();
        db = new DefaultContext(options, companyContext);
        handler = new DeleteProductHandler(db);
    }

    public void Dispose()
    {
        db.Dispose();
    }

    [Fact]
    public async Task Handle_WhenProductExists_SetsDeletedDate()
    {
        // Arrange
        var productId = Guid.NewGuid();
        var product = new ProductModel
        {
            Id = productId,
            Name = "Product",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        db.BasicProducts.Add(product);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCommand(productId);
        var beforeDelete = DateTime.Now;

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProduct = await db.BasicProducts.FindAsync([productId], CancellationToken.None);
        Assert.NotNull(deletedProduct);
        Assert.NotNull(deletedProduct.Deleted);
        Assert.True(deletedProduct.Deleted >= beforeDelete.AddSeconds(-1));
        Assert.True(deletedProduct.Deleted <= DateTime.Now.AddSeconds(1));
    }

    [Fact]
    public async Task Handle_WhenProductDoesNotExist_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var products = await db.BasicProducts.ToListAsync();
        Assert.Empty(products);
    }

    [Fact]
    public async Task Handle_WithMultipleProducts_OnlyDeletesSpecified()
    {
        // Arrange
        var product1Id = Guid.NewGuid();
        var product2Id = Guid.NewGuid();

        var product1 = new ProductModel
        {
            Id = product1Id,
            Name = "Product 1",
            CostPrice = 10.00m,
            SalesPrice = 20.00m,
            Quantity = 100,
            CategoryId = Guid.NewGuid()
        };

        var product2 = new ProductModel
        {
            Id = product2Id,
            Name = "Product 2",
            CostPrice = 15.00m,
            SalesPrice = 25.00m,
            Quantity = 50,
            CategoryId = Guid.NewGuid()
        };

        db.BasicProducts.AddRange(product1, product2);
        await db.SaveChangesAsync(CancellationToken.None);

        var command = new DeleteProductCommand(product1Id);

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var deletedProduct = await db.BasicProducts.FindAsync([product1Id], CancellationToken.None);
        var notDeletedProduct = await db.BasicProducts.FindAsync([product2Id], CancellationToken.None);

        Assert.NotNull(deletedProduct);
        Assert.NotNull(deletedProduct.Deleted);
        Assert.NotNull(notDeletedProduct);
        Assert.Null(notDeletedProduct.Deleted);
    }

    [Fact]
    public async Task Handle_WithEmptyDatabase_DoesNothing()
    {
        // Arrange
        var command = new DeleteProductCommand(Guid.NewGuid());

        // Act
        await handler.Handle(command, CancellationToken.None);

        // Assert
        var products = await db.BasicProducts.ToListAsync();
        Assert.Empty(products);
    }
}
